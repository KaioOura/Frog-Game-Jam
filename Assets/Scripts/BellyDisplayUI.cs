using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BellyDisplayUI : MonoBehaviour
{
    public event Action<int> OnSlotClicked;
    public event Action OnRemoveMeal;
    
    public Image[] bellySlot;
    public Image mealImage;
    
    public void Initialize(BellyFrog bellyFrog)
    {
        bellyFrog.OnUpdateBellyUI += UpdateUI;
        OnSlotClicked += bellyFrog.ThrowIngredient;
        OnRemoveMeal += bellyFrog.ThrowUpAllIngredients;
        
        UpdateUI(bellyFrog.BellyInventory.Belly, bellyFrog.activeMealSo);
    }
    
    public void UpdateMealUI(MealSo mealSo)
    {
        if (mealSo == null)
        {
            mealImage.gameObject.SetActive(false);
            return;
        }
        mealImage.gameObject.SetActive(true);
        mealImage.sprite = mealSo.image;
    }

    private void UpdateUI(List<Ingredient> ingredient, MealSo mealSo)
    {
        UpdateMealUI(mealSo);
        
        for (int i = 0; i < ingredient.Count; i++)
        {
            if (ingredient[i] == null)
            {
                bellySlot[i].gameObject.SetActive(false);
                continue;
            }
            
            bellySlot[i].gameObject.SetActive(true);
            bellySlot[i].sprite = ingredient[i].IngredientSo.myImage;
        }
    }

    public void OnItemClicked(int slot)
    {
        OnSlotClicked?.Invoke(slot);   
    }

    public void OnClickRemoveMeal()
    {
        OnRemoveMeal?.Invoke();
    }
}
