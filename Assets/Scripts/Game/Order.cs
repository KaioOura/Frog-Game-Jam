using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Serialization;

public class Order : MonoBehaviour
{
    [FormerlySerializedAs("myMeal")] public MealSo myMealSo;

    public Image mealImage;
    public Image[] recipeIngredientsIMG;
    public int mealTime;
    int orignalMealTime;

    public Image timeCount;
    
    private IEnumerator countDownRoutine;
    
    public void InitializeOrder(MealSo mealSo)
    {
        myMealSo = mealSo;
        mealImage.sprite = myMealSo.image;
        mealTime = mealSo.timeSecondsToPrepare;
        orignalMealTime = mealSo.timeSecondsToPrepare;

        for (int i = 0; i < mealSo.recipeIngredientsSo.Length; i++)
        {
            recipeIngredientsIMG[i].gameObject.SetActive(true);
            recipeIngredientsIMG[i].sprite = mealSo.recipeIngredientsSo[i].myImage;
        }

        if (countDownRoutine != null)
            StopCoroutine(countDownRoutine);

        countDownRoutine = TimeCountDown();
        StartCoroutine(countDownRoutine);
    }

    IEnumerator TimeCountDown()
    {
        while(mealTime >= 0)
        {
            if (GameManager.instance.gameStates == GameStates.finish)
            {
                yield break;
            }

            yield return new WaitForSeconds(1);

            mealTime -= 1;
            timeCount.fillAmount = GetTimeRemainingNormalized();
            //Debug.Log(timeCount);
        }

        GameManager.instance.ChangeLife(-2);
        RemoveOrder();
    }

    public float GetTimeRemainingNormalized()
    {
        return mealTime / (float)orignalMealTime;
    }

    public bool IsCloseToExpire()
    {
        return GetTimeRemainingNormalized() <= myMealSo.expirePercentage;
    }
    
    public void RemoveOrder()
    {
        OrderManager.instance.RemoveOrderFromList(this);
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
