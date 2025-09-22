using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Serialization;

public class Order : MonoBehaviour
{
    public Action<Order> OnOrderExpired;
    public Action<Order> OnReleaseToPool;

    public Image[] RecipeIngredientsIMG => recipeIngredientsIMG;
    public MealSo MyMealSo => myMealSo;
    public RectTransform Rect => _rect;
    
    [FormerlySerializedAs("myMeal")] public MealSo myMealSo;
    [SerializeField] private Image mealImage;
    [SerializeField] private Image[] recipeIngredientsIMG;
    [SerializeField] private GameObject[] ingredientSeparators;
    [SerializeField] private int mealTime;
    [SerializeField] private int orignalMealTime;
    [SerializeField] private Image timeCount;
    [SerializeField] private Image ingredientGrid;

    private IEnumerator countDownRoutine;
    private OrderHighlighter _orderHighlighter;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }
    
    public void SetOrderHighlighter(OrderHighlighter orderHighlighter)
    {
        _orderHighlighter = orderHighlighter;
    }
    
    public void InitializeOrder(MealSo mealSo, OrderManager orderManager)
    {
        myMealSo = mealSo;
        mealImage.sprite = myMealSo.image;
        mealTime = mealSo.timeSecondsToPrepare;
        orignalMealTime = mealSo.timeSecondsToPrepare;

        timeCount.fillAmount = GetTimeRemainingNormalized();

        OnOrderExpired = null;
        OnOrderExpired += orderManager.ReceiveOrderExpired;

        for (int i = 0; i < mealSo.recipeIngredientsSo.Length; i++)
        {
            recipeIngredientsIMG[i].gameObject.SetActive(true);
            recipeIngredientsIMG[i].sprite = mealSo.recipeIngredientsSo[i].myImage;

            int separatorIndex = i - 1;
            if (separatorIndex >= 0)
                ingredientSeparators[separatorIndex].SetActive(true);
        }

        _orderHighlighter.UpdateOrderIngredients();
        
        if (countDownRoutine != null)
            StopCoroutine(countDownRoutine);

        countDownRoutine = TimeCountDown();
        StartCoroutine(countDownRoutine);
    }

    IEnumerator TimeCountDown()
    {
        yield return new WaitForEndOfFrame();

        float timeRemaining = 1;
        timeCount.color = GetTimeColor(timeRemaining);
        
        while (mealTime >= 0)
        {
            if (GameManager.instance.gameStates == GameStates.finish)
            {
                yield break;
            }

            yield return new WaitForSeconds(1);

            mealTime -= 1;
            timeRemaining = GetTimeRemainingNormalized();
            timeCount.color = GetTimeColor(timeRemaining);
            timeCount.fillAmount = timeRemaining;
            //Debug.Log(timeCount);
        }

        OnOrderExpired?.Invoke(this);
    }

    private Color GetTimeColor(float timeRemaining)
    {
        return timeRemaining switch
        {
            > 0.7f => Color.green,
            > 0.3f => Color.yellow,
            _ => Color.red
        };
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
        foreach (var t in recipeIngredientsIMG)
        {
            t.gameObject.SetActive(false);
        }

        foreach (var ingredientSeparator in ingredientSeparators)
        {
            ingredientSeparator.SetActive(false);
        }
        
        StopAllCoroutines();
        OnReleaseToPool?.Invoke(this);
    }
}