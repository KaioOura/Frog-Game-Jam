using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

public class OrderManager : MonoBehaviour
{
    public Action<IngredientSo[]> OnRemoveOrder;
    public Action<int> OnOrderExpired;
    public Action OnMealDeliveredSuccessfully;
    public Action<MealSo> OnOrderSpawned;
    
    public List<Order> ActiveOrders => _activeOrders;
    public string LastMealExpired => _lastMealExpired;

    [SerializeField] private GameObject sapo;
    [FormerlySerializedAs("layoutanim")] [SerializeField] private Animator layoutAnim;
    [FormerlySerializedAs("boomboxanim")] [SerializeField] private Animator boomboxAnim;
    
    [SerializeField] private AudioClip successOrder;
    
    [SerializeField] private MealSo[] meals;

    [SerializeField] private int maxOrders;
    [SerializeField] private int difficultyWeightMultiplier = 3;
    
    [Tooltip("Time in seconds to spawn order based on difficulty")]
    [SerializeField] private float[] timeToSpawn;
    
    [FormerlySerializedAs("lastOrderMeal")]
    [SerializeField] private MealSo lastOrderMealSo;
    
    [SerializeField] private Transform ordersPos;
    [SerializeField] private VerticalUIList verticalUIList;

    private List<Order> _activeOrders = new List<Order>();
    private float _timeSpawn;
    private ObjectPoolManager _objectPoolManager;
    private ScoreManager _scoreManager;
    private DifficultyManager _difficultyManager;
    private MealSo _currentMatchedMeal;
    private Order _currentDeliveredOrder;
    private Dictionary<Difficulty, List<MealSo>> _mealsByDifficulty = new Dictionary<Difficulty, List<MealSo>>();
    private string _lastMealExpired;
    
    private List<MealSo> _eligibleMealsPool = new List<MealSo>(10);
    
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
        
        if (selectedMeal == null) selectedMeal = _eligibleMealsPool[0];

        if (selectedMeal == lastOrderMealSo && _eligibleMealsPool.Count > 1)
        {
            // Pega o próximo da lista de forma circular (muito performático)
            int index = _eligibleMealsPool.IndexOf(selectedMeal);
            selectedMeal = _eligibleMealsPool[(index + 1) % _eligibleMealsPool.Count];
        }

        ExecuteSpawn(selectedMeal);
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
                    
                // Definir o peso: Peso base + bônus se for da dificuldade atual
                // Isso faz com que pratos da dificuldade atual sejam os protagonistas
                int weight = (m.difficulty == currentDifficulty) ? m.weight * difficultyWeightMultiplier : m.weight;
                    
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
            int weight = (m.difficulty == currentDifficulty) ? m.weight * difficultyWeightMultiplier : m.weight;
            cursor += weight;

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
        verticalUIList.AddUI(order.Rect);
        order.transform.SetParent(ordersPos);
        order.transform.localScale = Vector3.one;

        order.InitializeOrder(mealSo, this);
        lastOrderMealSo = mealSo;
        _activeOrders.Add(order);
        OnOrderSpawned?.Invoke(mealSo);
    }
    
    private void OnSuccessMealDelivered(MealSo mealSo)
    {
        layoutAnim.SetTrigger("Success");
        boomboxAnim.SetTrigger("Pulo");
        _scoreManager.AddScore(mealSo.score);
        
        OnMealDeliveredSuccessfully?.Invoke();
        //Debug.Log("Pontua��o!");
    }

    public void OnMealDelivered(MealSo mealSo)
    {
        if (_currentMatchedMeal == mealSo)
        {
            OnSuccessMealDelivered(mealSo);
        }
        
        RemoveOrderFromList(_currentDeliveredOrder);
    }

    public void CheckMatchMeal(MealSo mealSo)
    {
        for (int i = 0; i < _activeOrders.Count; i++)
        {
            if (_activeOrders[i].myMealSo == mealSo)
            {
                Order item = _activeOrders[i];
                item.StopAllCoroutines();
                _currentMatchedMeal = mealSo;
                _currentDeliveredOrder = item;
                AudioManager.instance.PlayAudioOneShot(successOrder);
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
            OnRemoveOrder?.Invoke(order.myMealSo.recipeIngredientsSo);
            _activeOrders.Remove(order);
            DeleteOrder(order);
        }
    }

    private void DeleteOrder(Order order)
    {
        order.DeleteOrder();
        verticalUIList.RemoveUI(order.Rect);
    }
}