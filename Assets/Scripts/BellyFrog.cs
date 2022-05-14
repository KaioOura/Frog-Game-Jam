using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BellyFrog : MonoBehaviour
{
    public Animator frogController;
    public BellyDisplay bellyDisplay;
    public List<IngredientScriptable> belly;
    public int maxIngredients;
    public Transform bellyPos;

    public List<Meal> meals;

    public Meal activeMeal;

    public Action OnIngredientAdded;

    bool isThrowingUp;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void AddIngredient(IngredientScriptable ingredient)
    {
        if (belly.Count - 1 >= maxIngredients)
            return;

        belly.Add(ingredient);
        ingredient.gameObject.SetActive(false);
        ingredient.transform.SetParent(bellyPos.transform);
        ingredient.transform.localPosition = Vector3.zero;

        bellyDisplay.UpdateUI();
        OnIngredientAdd();
    }

    public void ThrowUpAllIngredients()
    {
        if (isThrowingUp)
            return;

        StartCoroutine(ThrowUpIngredients());
    }

    IEnumerator ThrowUpIngredients()
    {
        isThrowingUp = true;
        int numIngredients = belly.Count - 1;

        if (activeMeal != null)
        {
            foreach (var item in belly)
            {
                Destroy(item.gameObject);
            }

            //Spawnar e lancar meal
            MealGO mealGO = Instantiate(activeMeal.mealGO);
            mealGO.transform.position = bellyPos.transform.position;
            frogController.SetTrigger("Food Out");
            mealGO.LaunchItSelf(transform.forward);
            activeMeal = null;
        }
        else
        {
            while (numIngredients >= 0)
            {
                frogController.SetTrigger("Food Out");
                LaunchIngredient(belly[numIngredients]);
                numIngredients--;
                yield return new WaitForSeconds(0.17f);
            }
        }

        
        belly.Clear();
        bellyDisplay.UpdateUI();
        bellyDisplay.UpdateMealUI(null);

        isThrowingUp = false;
    }

    void LaunchIngredient(IngredientScriptable ingredient)
    {
        ingredient.gameObject.SetActive(true);
        ingredient.transform.SetParent(null);
        ingredient.LaunchItSelf(transform.forward);
    }

    void LaunchMealGO(MealGO mealGo)
    {
        mealGo.LaunchItSelf(transform.forward);
    }

    public bool IsBellyFull()
    {
        return belly.Count - 1 == maxIngredients;
    }

    void OnIngredientAdd()
    {
        if (belly.Count < 2)
        {
            Debug.Log("Not a meal");
            return;
        }


        activeMeal = GetMeal();

        bellyDisplay.UpdateMealUI(activeMeal);

        if (activeMeal != null)
        {
            Debug.Log(activeMeal.name);
        }
        else
        {
            Debug.Log("Not a meal");
        }

    }

    Meal GetMeal()
    {
        Meal meal = null;

        foreach (var item in meals)
        {
            if (item.Match(belly))
            {
                meal = item;
                break;
            }
        }

        return meal;
    }

}
