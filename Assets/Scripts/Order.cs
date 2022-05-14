using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class Order : MonoBehaviour
{
    public Meal myMeal;

    public Image mealImage;
    public Image[] recipeIngredientsIMG;
    public int mealTime;
    int orignalMealTime;

    public Image timeCount;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeOrder(Meal meal)
    {
        myMeal = meal;
        mealImage.sprite = myMeal.image;
        mealTime = meal.timeSecondsToPrepare;
        orignalMealTime = meal.timeSecondsToPrepare;

        for (int i = 0; i < meal.recipeIngredients.Length; i++)
        {
            recipeIngredientsIMG[i].gameObject.SetActive(true);
            recipeIngredientsIMG[i].sprite = meal.recipeIngredients[i].ingredientScriptable.myImage;
        }

        StartCoroutine(TimeCountDown());

    }

    IEnumerator TimeCountDown()
    {
        while(mealTime >= 0)
        {
            yield return new WaitForSeconds(1);
            mealTime -= 1;
            timeCount.fillAmount = (float)mealTime / (float)orignalMealTime;
            Debug.Log(timeCount);
        } 
    }

    public void RemoveOrder()
    {
        Destroy(gameObject);
    }
}
