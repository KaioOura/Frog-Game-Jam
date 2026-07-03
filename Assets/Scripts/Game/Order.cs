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
    [SerializeField] private int orignalMealTime;
    [SerializeField] private Image timeCount;
    [SerializeField] private Image ingredientGrid;

    private float _timeRemaining;
    private int _urgentCursor;
    private bool _isFrozen;
    private IEnumerator countDownRoutine;
    private OrderHighlighter _orderHighlighter;
    private RectTransform _rect;

    public void SetFrozen(bool frozen) => _isFrozen = frozen;

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
        orignalMealTime = mealSo.timeSecondsToPrepare;
        _timeRemaining = orignalMealTime;
        _urgentCursor = 0;
        _isFrozen = false;

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

        while (_timeRemaining > 0)
        {
            if (GameManager.instance.gameStates == GameStates.finish)
            {
                yield break;
            }

            if (!_isFrozen)
            {
                _timeRemaining -= Time.deltaTime;

                float timeRemaining = GetTimeRemainingNormalized();
                timeCount.fillAmount = timeRemaining;
                timeCount.color = GetTimeColor(timeRemaining);
            }

            yield return null;
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
        return Mathf.Clamp01(_timeRemaining / orignalMealTime);
    }

    public bool IsCloseToExpire()
    {
        return GetTimeRemainingNormalized() <= myMealSo.expirePercentage;
    }

    // Cada order cicla os próprios ingredientes urgentes (estado por order,
    // resetado no InitializeOrder via pool). Evita que orders urgentes
    // simultâneas misturem ciclos.
    public IngredientSo GetNextUrgentIngredient()
    {
        var recipe = myMealSo.recipeIngredientsSo;
        IngredientSo next = recipe[_urgentCursor % recipe.Length];
        _urgentCursor++;
        return next;
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