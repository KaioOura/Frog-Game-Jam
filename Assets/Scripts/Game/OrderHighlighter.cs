using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OrderHighlighter : MonoBehaviour
{
    private BellyFrog _bellyFrog;
    private Order _order;
    
    private Image[] _orderIngredientsIMG;
    
    private List<IngredientSo> _cachedIngredients;

    public void Initialize(Order order, BellyFrog bellyFrog)
    {
        _order = order;

        _orderIngredientsIMG = _order.RecipeIngredientsIMG;
        _bellyFrog = bellyFrog;
    }

    public void UpdateOrderIngredients()
    {
        ResetIngredientsColor();


        foreach (var ingredientSo in _bellyFrog.bellySo)
        {
            HighLightIngredients(ingredientSo);
        }
    }
    
    public void HighLightIngredients(IngredientSo ingredientSo)
    {
        if (!gameObject.activeInHierarchy) return;
        
        if (!_order.MyMealSo.recipeIngredientsSo.Contains(ingredientSo)) return;

        
        int index = System.Array.IndexOf(_order.MyMealSo.recipeIngredientsSo, ingredientSo);
        
        _orderIngredientsIMG[index].color = Color.green;
    }

    public void ResetIngredientsColor()
    {
        for (int i = 0; i < _orderIngredientsIMG.Length; i++)
        {
            _orderIngredientsIMG[i].color = Color.white;
        }
    }
}
