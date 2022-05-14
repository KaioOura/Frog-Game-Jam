using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class BellyFrog : MonoBehaviour
{
    public Animation_Controller animationController;
    public Animator frogController;
    public BellyDisplay bellyDisplay;
    public List<IngredientScriptable> belly;
    public int maxIngredients;
    public Transform bellyPos;

    public List<Meal> meals;

    public Meal activeMeal;
    GameObject mealGO;

    public Action OnIngredientAdded;

    bool isThrowingUp;

    public Transform cartPos;

    // Start is called before the first frame update
    void Start()
    {
        animationController = GetComponent<Animation_Controller>();
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
        animationController.realayerWeight += 0.25f;
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

    public void MoveMealToCart()
    {
        mealGO.transform.DOMove(cartPos.position, 0.2f).OnComplete(() =>
        {
            OrderManager.instance.CheckMeal(activeMeal);
            activeMeal = null;
            mealGO = null;
        });
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
            MealGO _mealGO = Instantiate(activeMeal.mealGO);
            mealGO = _mealGO.gameObject;
            _mealGO.transform.position = bellyPos.transform.position;
            frogController.SetTrigger("Food Out");
            animationController.realayerWeight = 0;

            MoveMealToCart();

            //_mealGO.LaunchItSelf(transform.forward);

        }
        else
        {
            while (numIngredients >= 0)
            {
                animationController.realayerWeight -= 0.25f;
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
