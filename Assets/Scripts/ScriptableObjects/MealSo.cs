using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Meal", menuName = "Meal", order = 1)]
public class MealSo : ScriptableObject
{
    public string mealName;
    public int score;
    public Sprite image;
    public int timeSecondsToPrepare;
    [Range(0, 1)]
    public float expirePercentage;

    public MealGO mealGO;
    public Difficulty difficulty;

    [FormerlySerializedAs("recipeIngredients")] public IngredientSo[] recipeIngredientsSo;
    
    public bool Match(List<Ingredient> ingredients)
    {
        if (ingredients.Count <= 1)
        {
            Debug.Log("NOT MATCH: Meal need more than " + ingredients.Count+" ingredients");
            return false;
        }

        if (recipeIngredientsSo.Length != ingredients.Count)
        {
            Debug.Log("NOT MATCH: Meal ingeredients count not matching");
            return false;
        }

        List<bool> isMatch = new List<bool>();

        for (int i = 0; i < ingredients.Count; i++)
        {
            isMatch.Add(false);
        }

        for (int i = 0; i < recipeIngredientsSo.Length; i++)
        {
            for (int y = 0; y < ingredients.Count; y++)
            {
                if (recipeIngredientsSo[i] == ingredients[y].IngredientSo)
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
