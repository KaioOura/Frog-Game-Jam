using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Meal", menuName = "Meal", order = 1)]
public class Meal : ScriptableObject
{
    public string mealName;
    public int score;
    public Sprite image;
    public int timeSecondsToPrepare;

    public MealGO mealGO;

    public enum MyMeal {macarrao_queijo, esfiha, macarrao_camarao, hamburguer_camarao, hamburguer, macarrao_almondegas, pizza_peperoni, pizza_cogumelo }
    public MyMeal myMeal;

    public enum Difficulty { easy, normal, hard}
    public Difficulty difficulty;

    public IngredientBase[] recipeIngredients;
    
    public bool Match(List<IngredientScriptable> ingredientScriptables)
    {
        if (ingredientScriptables.Count <= 1)
        {
            Debug.Log("NOT MATCH: Meal need more than " + ingredientScriptables.Count+" ingredients");
            return false;
        }

        if (recipeIngredients.Length != ingredientScriptables.Count)
        {
            Debug.Log("NOT MATCH: Meal ingeredients count not matching");
            return false;
        }

        List<bool> isMatch = new List<bool>();

        for (int i = 0; i < ingredientScriptables.Count; i++)
        {
            isMatch.Add(false);
        }

        for (int i = 0; i < recipeIngredients.Length; i++)
        {
            for (int y = 0; y < ingredientScriptables.Count; y++)
            {
                if (recipeIngredients[i].ingredientEnum == ingredientScriptables[y].ingredient.ingredientEnum)
                {
                    isMatch[i] = true;
                    break;
                }
            }

        }

        return isMatch.TrueForAll(IsMatch);
    }

    bool IsMatch(bool isMatch = true)
    {
        return isMatch;
    }

}
