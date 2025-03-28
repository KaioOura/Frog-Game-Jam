using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class OrderManager : MonoBehaviour
{

    public GameObject sapo;
    public GameObject StarVFX_GO;
    public Animator layoutanim, boomboxanim;
    public static OrderManager instance;

    public Meal[] meals;
    public List<Order> activeOrders;

    public int maxOrders;

    public int[] difficultyBreakdown;
    public float[] timeToSpawn;
    float timeSpawn;

    public int difficultyIndex;

    public static float timeTracker;

    public Order orderGO;
    public Meal lastOrderMeal;


    public Transform ordersPos;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (GameManager.instance.gameStates != GameManager.GameStates.game)
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            SpawnOrder(0);
        }

    }


    public void SpawnOrder(int difficulty)
    {
        List<Meal> mealsAvailable = new List<Meal>();

        for (int i = 0; i < difficulty + 1; i++)
        {
            if (i == 0)
            {
                foreach (var item in meals)
                {
                    if (item.difficulty == Meal.Difficulty.easy)
                    {
                        mealsAvailable.Add(item);
                    }
                }
            }
            else if (i == 1)
            {
                foreach (var item in meals)
                {
                    if (item.difficulty == Meal.Difficulty.normal)
                    {
                        mealsAvailable.Add(item);
                    }
                }
            }
            else if (i == 2)
            {
                foreach (var item in meals)
                {
                    if (item.difficulty == Meal.Difficulty.hard)
                    {
                        mealsAvailable.Add(item);
                    }
                }
            }
        }

        //Spawnar order

        int randMeal = UnityEngine.Random.Range(0, mealsAvailable.Count);

        //Debug.Log($"Meal {mealsAvailable.Count}");

        if (mealsAvailable[randMeal] == lastOrderMeal)
        {
            SpawnOrder(difficultyIndex);
            return;
        }

        Order order = Instantiate(orderGO, ordersPos);
        order.transform.localPosition = new Vector2(order.transform.localPosition.x, 71);

        order.InitializeOrder(mealsAvailable[randMeal]);

        lastOrderMeal = mealsAvailable[randMeal];

        activeOrders.Add(order);

    }

    public void CheckMeal(Meal meal)
    {
        if (IsMealMatch(meal))
        {
            //Creditar pontos, feedbck de acerto, sumir com pedido
            layoutanim.SetTrigger("Success");
            boomboxanim.SetTrigger("Pulo");
            Instantiate(StarVFX_GO, sapo.transform.position,Quaternion.identity);
            GameManager.instance.AddScore(meal.score);
            Debug.Log("Pontua��o!");
        }
    }

    public void CheckMeal(Meal meal, bool stopTimer, Action action = null)
    {
        if (IsMealMatch(meal, true))
        {
            action?.Invoke();
            Debug.Log("Pontua��o!");
        }
    }

    public bool IsMealMatch(Meal meal)
    {
        bool isMatch = false;

        foreach (var item in activeOrders)
        {
            if (item.myMeal == meal)
            {
                isMatch = true;
                activeOrders.Remove(item);
                item.RemoveOrder();
                break;
            }
        }

        return isMatch;
    }

    bool IsMealMatch(Meal meal, bool stopTimer)
    {
        bool isMatch = false;

        foreach (var item in activeOrders)
        {
            if (item.myMeal == meal)
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
            activeOrders.Remove(order);
        }
    }

}
