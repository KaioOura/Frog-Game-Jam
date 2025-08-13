using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class OrderManager : MonoBehaviour
{
    public Action<IngredientSo[]> OnRemoveOrder;
    
    public GameObject sapo;
    public GameObject StarVFX_GO;
    public Animator layoutanim, boomboxanim;
    public static OrderManager instance;

    public MealSo[] meals;
    public List<Order> activeOrders;

    public int maxOrders;

    public int[] difficultyBreakdown;
    public float[] timeToSpawn;
    float timeSpawn;

    public int difficultyIndex;

    public static float timeTracker;

    public Order orderGO;
    [FormerlySerializedAs("lastOrderMeal")] public MealSo lastOrderMealSo;


    public Transform ordersPos;
    
    
    private Dictionary<Difficulty, List<MealSo>> _mealsByDifficulty = new Dictionary<Difficulty, List<MealSo>>();

    private void Awake()
    {
        instance = this;
    }

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

        order.InitializeOrder(mealsAvailable[randMeal]);

        lastOrderMealSo = mealsAvailable[randMeal];

        activeOrders.Add(order);

    }

    public void CheckMeal(MealSo mealSo)
    {
        if (IsMealMatch(mealSo))
        {
            //Creditar pontos, feedbck de acerto, sumir com pedido
            layoutanim.SetTrigger("Success");
            boomboxanim.SetTrigger("Pulo");
            Instantiate(StarVFX_GO, sapo.transform.position,Quaternion.identity);
            GameManager.instance.AddScore(mealSo.score);
            Debug.Log("Pontua��o!");
        }
    }

    public void CheckMeal(MealSo mealSo, bool stopTimer, Action action = null)
    {
        if (IsMealMatch(mealSo, true))
        {
            action?.Invoke();
            Debug.Log("Pontua��o!");
        }
    }

    public bool IsMealMatch(MealSo mealSo)
    {
        bool isMatch = false;

        foreach (var item in activeOrders)
        {
            if (item.myMealSo == mealSo)
            {
                isMatch = true;
                activeOrders.Remove(item);
                item.RemoveOrder();
                break;
            }
        }

        return isMatch;
    }

    bool IsMealMatch(MealSo mealSo, bool stopTimer)
    {
        bool isMatch = false;

        foreach (var item in activeOrders)
        {
            if (item.myMealSo == mealSo)
            {
                isMatch = true;
                item.StopAllCoroutines();
                break;
            }
        }

        return isMatch;
    }

    public void ResetOrders()
    {

        timeTracker = 0;
        timeSpawn = 0;
        difficultyIndex = 0;

        foreach (var item in activeOrders)
        {
            Destroy(item.gameObject);
        }

        activeOrders.Clear();
    }

    public void RemoveOrderFromList(Order order)
    {
        if (activeOrders.Contains(order))
        {
            OnRemoveOrder?.Invoke(order.myMealSo.recipeIngredientsSo);
            activeOrders.Remove(order);
        }
    }

}
