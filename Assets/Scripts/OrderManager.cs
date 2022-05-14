using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public Meal[] meals;

    public int[] difficultyBreakdown;

    public int difficultyIndex;

    public static float timeTracker;

    public Order orderGO;

    public Transform ordersPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timeTracker += Time.deltaTime;

        if (timeTracker < difficultyBreakdown[difficultyIndex])
        {
            //difficultyIndex++;
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
            //else if (i == 1)
            //{
            //    foreach (var item in meals)
            //    {
            //        if (item.difficulty == Meal.Difficulty.normal)
            //        {
            //            mealsAvailable.Add(item);
            //        }
            //    }
            //}
            //else if (i == 2)
            //{
            //    foreach (var item in meals)
            //    {
            //        if (item.difficulty == Meal.Difficulty.hard)
            //        {
            //            mealsAvailable.Add(item);
            //        }
            //    }
            //}
        }

        //Spawnar order

        int randMeal = Random.Range(0, mealsAvailable.Count);

        Debug.Log($"Meal {mealsAvailable.Count}");

        Order order = Instantiate(orderGO, ordersPos);

        order.InitializeOrder(mealsAvailable[0]);

    }
}
