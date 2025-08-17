using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    public Dictionary<IngredientSo, ObjectPool<Ingredient>> IngredientPool => _ingredientPool;
    
    [SerializeField] private List<IngredientSo> ingredientSos;
    [SerializeField] private Transform ingredientsParent;

    private Dictionary<IngredientSo, ObjectPool<Ingredient>> _ingredientPool;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _ingredientPool = new Dictionary<IngredientSo, ObjectPool<Ingredient>>();

        foreach (var ingredientSo in ingredientSos)
        {
            if (_ingredientPool.ContainsKey(ingredientSo)) continue;

            ObjectPool<Ingredient> ingredientPool = new ObjectPool<Ingredient>(
                createFunc: () => CreateIngredient(ingredientSo), OnGetIngredientFromPool, OnReleaseToPool,
                OnDestroyBullet, true, 13, 20);
            
            _ingredientPool.Add(ingredientSo, ingredientPool);
        }
    }

    private Ingredient CreateIngredient(IngredientSo ingredientSo)
    {
        var ingredient = Instantiate(ingredientSo.ingredientPrefab, ingredientsParent);
        ingredient.OnReleaseToPool = ReleaseIngredient;
        ingredient.gameObject.SetActive(false);
        return ingredient;
    }

    private void OnGetIngredientFromPool(Ingredient ingredient)
    {
        ingredient.gameObject.SetActive(true);
        //ingredient.transform.position = Vector3.zero; // exemplo
    }

    private void OnReleaseToPool(Ingredient ingredient)
    {
        ingredient.transform.SetParent(ingredientsParent);
        
        ingredient.gameObject.SetActive(false);
        ingredient.transform.rotation = Quaternion.identity;
        ingredient.transform.localScale = Vector3.one;
    }

    private void OnDestroyBullet(Ingredient ingredient)
    {
        Destroy(ingredient.gameObject);
    }

    private void ReleaseIngredient(Ingredient ingredient)
    {
        _ingredientPool[ingredient.IngredientSo].Release(ingredient);
    }
}