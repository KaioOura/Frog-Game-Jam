using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BellyInventory : MonoBehaviour
{
    public int MaxIngredients => maxIngredients;
    public List<Ingredient> Belly => _belly;
    
    [SerializeField] private int maxIngredients = 3;
    
    private List<Ingredient> _belly = new List<Ingredient>();
    private List<IngredientSo> _bellySo = new List<IngredientSo>();

    private void Awake()
    {
        for (int i = 0; i < maxIngredients; i++)
        {
            _belly.Add(null);
        }
    }

    public int GetBellyCount()
    {
        return _belly.Count(t => t != null);
    }

    public void AddIngredient(Ingredient ingredient)
    {
        for (int i = 0; i < _belly.Count; i++)
        {
            if (_belly[i] != null) continue;
            _belly[i] = ingredient;
            _bellySo.Add(ingredient.IngredientSo);
            return;

        }
    }

    public void RemoveIngredient(int slotIndex)
    {
        if (_belly[slotIndex] == null) return;
        
        _bellySo.Remove(_belly[slotIndex].IngredientSo);
        _belly[slotIndex] = null;
    }

    public void ClearBelly()
    {
        for (var index = 0; index < _belly.Count; index++)
        {
            _belly[index] = null;
        }

        _bellySo.Clear();
    }
    
    public List<IngredientSo> GetIngredientSOs()
    {
        return _bellySo;
    }

    public List<Ingredient> GetIngredients()
    {
        return _belly
            .Where(item => item != null)
            .ToList();
    }

    public bool IsFull()
    {
        return GetBellyCount() >= maxIngredients;
    }
}
