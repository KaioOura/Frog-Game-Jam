using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Order : MonoBehaviour
{
    public Meal myMeal;

    public Image mealImage;
    public Image[] recipeIngredientsIMG;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeOrder(Meal meal)
    {
        myMeal = meal;
        mealImage.sprite = myMeal.image;

        for (int i = 0; i < meal.recipeIngredients.Length; i++)
        {
            recipeIngredientsIMG[i].gameObject.SetActive(true);
            recipeIngredientsIMG[i].sprite = meal.recipeIngredients[i].ingredientScriptable.myImage;
        }

    }
}
