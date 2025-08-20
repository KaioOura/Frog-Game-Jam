using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class ObjectPoolManager : MonoBehaviour
{
    public IngredientPoolManager IngredientPoolManager => _ingredientPool;
    public OrderPoolManager OrderPool => _orderPool;
    public MealPoolManager MealPool => _mealPool;
    
    [SerializeField] private IngredientPoolManager _ingredientPool;
    [SerializeField] private OrderPoolManager _orderPool;
    [SerializeField] private MealPoolManager _mealPool;

    public void Initialize()
    {
        _ingredientPool.Initialize();
        _orderPool.Initialize();
        _mealPool.Initialize();
    }

   
}