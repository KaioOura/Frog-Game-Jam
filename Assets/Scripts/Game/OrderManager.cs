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

    public GameObject sapo;
    public GameObject StarVFX_GO;
    public Animator layoutanim, boomboxanim;
    [SerializeField] private AudioClip successOrder;
    
    public MealSo[] meals;
    public List<Order> activeOrders;

    public int maxOrders;

    public int[] difficultyBreakdown;
    public float[] timeToSpawn;
    float timeSpawn;

    public int difficultyIndex;

    public static float timeTracker;

    public Order orderGO;

    [FormerlySerializedAs("lastOrderMeal")]
    public MealSo lastOrderMealSo;


    public Transform ordersPos;

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

    public void Initialize(ScoreManager scoreManager)
    {
        _scoreManager = scoreManager;
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates != GameStates.game)
            return;

        timeTracker += Time.deltaTime;
        timeSpawn += Time.deltaTime;

        if (timeTracker > difficultyBreakdown[difficultyIndex] && difficultyIndex < difficultyBreakdown.Length - 1)
        {
            difficultyIndex++;
            timeTracker = 0;
        }

        if (timeSpawn > timeToSpawn[difficultyIndex] && activeOrders.Count < maxOrders)
        {
            SpawnOrder(difficultyIndex);
            timeSpawn = 0;
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

        Order order = Instantiate(orderGO, ordersPos);
        order.transform.localPosition = new Vector2(order.transform.localPosition.x, 71);

        order.InitializeOrder(mealsAvailable[randMeal], this);

        lastOrderMealSo = mealsAvailable[randMeal];

        activeOrders.Add(order);
    }

    private void OnSuccessMealDelivered(MealSo mealSo)
    {
        layoutanim.SetTrigger("Success");
        boomboxanim.SetTrigger("Pulo");
        Instantiate(StarVFX_GO, sapo.transform.position, Quaternion.identity);
        _scoreManager.AddScore(mealSo.score);
        Debug.Log("Pontua��o!");
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
        foreach (var item in activeOrders.Where(item => item.myMealSo == mealSo))
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
        timeSpawn = 0;
        difficultyIndex = 0;

        foreach (var item in activeOrders)
        {
            item.DeleteOrder();
        }

        activeOrders.Clear();
    }

    public void ReceiveOrderExpired(Order order)
    {
        OnOrderExpired?.Invoke(-2);
        RemoveOrderFromList(order);
    }
    
    public void RemoveOrderFromList(Order order)
    {
        if (activeOrders.Contains(order))
        {
            OnRemoveOrder?.Invoke(order.myMealSo.recipeIngredientsSo);
            activeOrders.Remove(order);
            order.DeleteOrder();
        }
    }
}