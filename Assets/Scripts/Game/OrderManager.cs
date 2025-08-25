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
    
    public List<Order> ActiveOrders => _activeOrders;

    [SerializeField] private GameObject sapo;
    [FormerlySerializedAs("layoutanim")] [SerializeField] private Animator layoutAnim;
    [FormerlySerializedAs("boomboxanim")] [SerializeField] private Animator boomboxAnim;
    
    [SerializeField] private AudioClip successOrder;
    
    [SerializeField] private MealSo[] meals;

    [SerializeField] private int maxOrders;

    [Tooltip("Time in seconds to change difficulty")]
    [SerializeField] private int[] difficultyBreakdown;
    
    [Tooltip("Time in seconds to spawn order based on difficulty")]
    [SerializeField] private float[] timeToSpawn;
    
    [SerializeField] private int difficultyIndex;
    
    [FormerlySerializedAs("lastOrderMeal")]
    [SerializeField] private MealSo lastOrderMealSo;
    
    [SerializeField] private Transform ordersPos;
    [SerializeField] private VerticalUIList verticalUIList;

    private List<Order> _activeOrders = new List<Order>();
    private float _timeSpawn;
    private static float timeTracker;
    private ObjectPoolManager _objectPoolManager;
    private ScoreManager _scoreManager;
    private MealSo _currentMatchedMeal;
    private Order _currentDeliveredOrder;
    private Dictionary<Difficulty, List<MealSo>> _mealsByDifficulty = new Dictionary<Difficulty, List<MealSo>>();
    
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

    public void Initialize(ScoreManager scoreManager, ObjectPoolManager objectPoolManager)
    {
        _scoreManager = scoreManager;
        _objectPoolManager = objectPoolManager;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates != GameStates.game)
            return;

        timeTracker += Time.deltaTime;
        _timeSpawn += Time.deltaTime;

        if (timeTracker > difficultyBreakdown[difficultyIndex] && difficultyIndex < difficultyBreakdown.Length - 1)
        {
            difficultyIndex++;
            timeTracker = 0;
        }

        if (_timeSpawn > timeToSpawn[difficultyIndex] && _activeOrders.Count < maxOrders)
        {
            SpawnOrder(difficultyIndex);
            _timeSpawn = 0;
        }

        // if (Input.GetKeyDown(KeyCode.R))
        // {
        //     SpawnOrder(0);
        // }
    }

    private void SpawnOrder(int difficulty)
    {
        List<MealSo> mealsAvailable = _mealsByDifficulty[(Difficulty)difficulty];

        int randMeal = UnityEngine.Random.Range(0, mealsAvailable.Count);

        //Debug.Log($"Meal {mealsAvailable.Count}");

        if (mealsAvailable[randMeal] == lastOrderMealSo)
        {
            SpawnOrder(difficultyIndex);
            return;
        }
        
        Order order = _objectPoolManager.OrderPool.Pool.Get();
        verticalUIList.AddUI(order.Rect);
        order.transform.SetParent(ordersPos);
        order.transform.localScale = Vector3.one;

        order.InitializeOrder(mealsAvailable[randMeal], this);

        lastOrderMealSo = mealsAvailable[randMeal];

        _activeOrders.Add(order);
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
        foreach (var item in _activeOrders.Where(item => item.myMealSo == mealSo))
        {
            item.StopAllCoroutines();
            _currentMatchedMeal = mealSo;
            _currentDeliveredOrder = item;
            AudioManager.instance.PlayAudioOneShot(successOrder);
            break;
        }
    }
    
    public void ResetOrders()
    {
        timeTracker = 0;
        _timeSpawn = 0;
        difficultyIndex = 0;

        foreach (var item in _activeOrders)
        {
            DeleteOrder(item);
        }

        _activeOrders.Clear();
    }

    public void ReceiveOrderExpired(Order order)
    {
        OnOrderExpired?.Invoke(2);
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