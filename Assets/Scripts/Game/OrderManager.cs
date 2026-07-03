using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class OrderManager : MonoBehaviour
{
    public Action<int> OnOrderExpired;
    public Action OnMealDeliveredSuccessfully;
    public Action<MealSo> OnOrderSpawned;

    public List<Order> ActiveOrders => _activeOrders;
    public string LastMealExpired => _lastMealExpired;

    [SerializeField] private OrderPresenter orderPresenter;

    [SerializeField] private MealSo[] meals;

    [SerializeField] private int maxOrders;
    [SerializeField] private int difficultyWeightMultiplier = 3;
    [Tooltip("Divisor aplicado ao peso da última meal sorteada para reduzir repetições sem padrão fixo")]
    [SerializeField] private int repeatPenaltyDivisor = 3;

    [Tooltip("Time in seconds to spawn order based on difficulty")]
    [SerializeField] private float[] timeToSpawn;

    [Tooltip("Tempo (segundos) que os timers dos pedidos ficam congelados após ver um ad de recompensa")]
    [SerializeField] private float postAdFreezeSeconds = 5f;

    [FormerlySerializedAs("lastOrderMeal")]
    [SerializeField] private MealSo lastOrderMealSo;

    [SerializeField] private Transform ordersPos;

    private List<Order> _activeOrders = new List<Order>();
    private float _timeSpawn;
    private ObjectPoolManager _objectPoolManager;
    private ScoreManager _scoreManager;
    private DifficultyManager _difficultyManager;
    private Order _currentDeliveredOrder;
    private Dictionary<Difficulty, List<MealSo>> _mealsByDifficulty = new Dictionary<Difficulty, List<MealSo>>();
    private string _lastMealExpired;
    
    private List<MealSo> _eligibleMealsPool = new List<MealSo>(10);
    private bool _ordersFrozen;
    private Coroutine _freezeRoutine;
    
    // Start is called before the first frame update
    void Start()
    {
        _mealsByDifficulty.Add(Difficulty.easy, new List<MealSo>());
        _mealsByDifficulty.Add(Difficulty.normal, new List<MealSo>());
        _mealsByDifficulty.Add(Difficulty.hard, new List<MealSo>());

        foreach (var meal in meals)
        {
            if (_mealsByDifficulty.ContainsKey(meal.difficulty))
            {
                _mealsByDifficulty[meal.difficulty].Add(meal);
            }
        }
    }

    public void Initialize(ScoreManager scoreManager, ObjectPoolManager objectPoolManager, DifficultyManager difficultyManager)
    {
        _scoreManager = scoreManager;
        _objectPoolManager = objectPoolManager;
        _difficultyManager = difficultyManager; 
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates != GameStates.game)
            return;
        
        _timeSpawn += Time.deltaTime;

        if (_timeSpawn > timeToSpawn[(int)_difficultyManager.GetDifficulty()] && _activeOrders.Count < maxOrders)
        {
            SpawnOrder(_difficultyManager.GetDifficulty());
            _timeSpawn = 0;
        }
    }

    private void SpawnOrder(Difficulty currentDifficulty)
    {
        var selectedMeal = GetMealByWeight(currentDifficulty);

        if (selectedMeal == null) return;

        ExecuteSpawn(selectedMeal);
    }

    // Reduz o peso da última meal sorteada para que repetições sejam raras e
    // sem padrão fixo, em vez de bloquear repetição de forma determinística.
    private int GetWeight(MealSo meal, Difficulty currentDifficulty)
    {
        int weight = (meal.difficulty == currentDifficulty)
            ? meal.weight * difficultyWeightMultiplier
            : meal.weight;

        if (meal == lastOrderMealSo)
            weight = Mathf.Max(1, weight / repeatPenaltyDivisor);

        return weight;
    }

    private MealSo GetMealByWeight(Difficulty currentDifficulty)
    {
        _eligibleMealsPool.Clear();
        int totalWeight = 0;
        int currentDiffInt = (int)currentDifficulty;

        // 1. Coletar todas as meals de dificuldades permitidas (atual ou menores)
        // Percorremos o dicionário que você já preencheu no Start
        foreach (var entry in _mealsByDifficulty)
        {
            if ((int)entry.Key > currentDiffInt) continue;
            
            List<MealSo> list = entry.Value;
            for (int i = 0; i < list.Count; i++)
            {
                MealSo m = list[i];

                // Peso base + bônus se for da dificuldade atual (e penalidade se
                // for a última meal sorteada). Pratos da dificuldade atual viram
                // protagonistas.
                int weight = GetWeight(m, currentDifficulty);

                if (weight <= 0) continue;

                _eligibleMealsPool.Add(m);
                totalWeight += weight;
            }
        }

        if (_eligibleMealsPool.Count == 0) return null;

        // 2. Sorteio com base no peso total
        int randomNumber = UnityEngine.Random.Range(0, totalWeight);
        int cursor = 0;
        MealSo selectedMeal = null;

        for (int i = 0; i < _eligibleMealsPool.Count; i++)
        {
            MealSo m = _eligibleMealsPool[i];
            cursor += GetWeight(m, currentDifficulty);

            if (randomNumber < cursor)
            {
                selectedMeal = m;
                break;
            }
        }

        return selectedMeal;
    }
    
    private void ExecuteSpawn(MealSo mealSo)
    {
        Order order = _objectPoolManager.OrderPool.Pool.Get();
        orderPresenter.AddOrder(order.Rect);
        order.transform.SetParent(ordersPos);
        order.transform.localScale = Vector3.one;

        order.InitializeOrder(mealSo, this);

        // Pedidos que nascem dentro da janela de freeze também começam congelados.
        if (_ordersFrozen)
            order.SetFrozen(true);

        lastOrderMealSo = mealSo;
        _activeOrders.Add(order);
        OnOrderSpawned?.Invoke(mealSo);
    }

    // Congela os timers de todos os pedidos ativos por postAdFreezeSeconds
    // (chamado após o jogador assistir a um ad de recompensa / segunda chance).
    public void FreezeActiveOrders()
    {
        if (_freezeRoutine != null)
            StopCoroutine(_freezeRoutine);

        _freezeRoutine = StartCoroutine(FreezeOrdersRoutine(postAdFreezeSeconds));
    }

    private IEnumerator FreezeOrdersRoutine(float seconds)
    {
        _ordersFrozen = true;
        foreach (var order in _activeOrders)
            order.SetFrozen(true);

        yield return new WaitForSeconds(seconds);

        foreach (var order in _activeOrders)
            order.SetFrozen(false);
        _ordersFrozen = false;
        _freezeRoutine = null;
    }
    
    private void OnSuccessMealDelivered(MealSo mealSo)
    {
        orderPresenter.PlaySuccessFeedback();
        _scoreManager.AddScore(mealSo.score);

        OnMealDeliveredSuccessfully?.Invoke();
    }

    public void OnMealDelivered(MealSo mealSo)
    {
        if (_currentDeliveredOrder != null && _currentDeliveredOrder.myMealSo == mealSo)
        {
            OnSuccessMealDelivered(mealSo);
        }

        RemoveOrderFromList(_currentDeliveredOrder);
        _currentDeliveredOrder = null;
    }

    public void CheckMatchMeal(MealSo mealSo)
    {
        for (int i = 0; i < _activeOrders.Count; i++)
        {
            if (_activeOrders[i].myMealSo == mealSo)
            {
                Order item = _activeOrders[i];
                item.StopAllCoroutines();
                _currentDeliveredOrder = item;
                orderPresenter.PlayMatchSound();
                break;
            }
        }
    }
    
    public void ResetOrders()
    {
        _timeSpawn = 0;

        foreach (var item in _activeOrders)
        {
            DeleteOrder(item);
        }

        _activeOrders.Clear();
    }

    public void ReceiveOrderExpired(Order order)
    {
        _lastMealExpired = order.myMealSo.name;
        OnOrderExpired?.Invoke(1);
        RemoveOrderFromList(order);
    }
    
    public void RemoveOrderFromList(Order order)
    {
        if (_activeOrders.Contains(order))
        {
            _activeOrders.Remove(order);
            DeleteOrder(order);
        }
    }

    private void DeleteOrder(Order order)
    {
        order.DeleteOrder();
        orderPresenter.RemoveOrder(order.Rect);
    }
}