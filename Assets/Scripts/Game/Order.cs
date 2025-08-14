using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Serialization;

public class Order : MonoBehaviour
{
    public Action<Order> OnOrderExpired;
    [FormerlySerializedAs("myMeal")] public MealSo myMealSo;

    public Image mealImage;
    public Image[] recipeIngredientsIMG;
    public int mealTime;
    int orignalMealTime;

    public Image timeCount;
    
    private IEnumerator countDownRoutine;
    
    public void InitializeOrder(MealSo mealSo, OrderManager orderManager)
    {
        myMealSo = mealSo;
        mealImage.sprite = myMealSo.image;
        mealTime = mealSo.timeSecondsToPrepare;
        orignalMealTime = mealSo.timeSecondsToPrepare;
        
        OnOrderExpired = null;
        OnOrderExpired += orderManager.ReceiveOrderExpired;

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
        
        OnOrderExpired?.Invoke(this);
    }

    public float GetTimeRemainingNormalized()
    {
        return mealTime / (float)orignalMealTime;
    }

    public bool IsCloseToExpire()
    {
        return GetTimeRemainingNormalized() <= myMealSo.expirePercentage;
    }
    
    public void DeleteOrder()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
