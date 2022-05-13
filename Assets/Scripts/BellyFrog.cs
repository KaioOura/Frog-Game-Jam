using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BellyFrog : MonoBehaviour
{
    public List<IngredientScriptable> belly;
    public int maxIngredients;
    public Transform bellyPos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddIngredient(IngredientScriptable ingredient)
    {
        if (belly.Count - 1 >= maxIngredients)
            return;

        belly.Add(ingredient);
        ingredient.gameObject.SetActive(false);
        ingredient.transform.SetParent(bellyPos.transform);
        ingredient.transform.localPosition = Vector3.zero;
    }

    public void ThrowUpAllIngredients()
    {
        StartCoroutine(ThrowUpIngredients());
    }

    IEnumerator ThrowUpIngredients()
    {
        int numIngredients = belly.Count - 1;

        while(numIngredients >= 0)
        {
            LaunchIngredient(belly[numIngredients]);
            numIngredients--;
            yield return new WaitForSeconds(0.15f);
        }

        belly.Clear();
    }

    void LaunchIngredient(IngredientScriptable ingredient)
    {
        ingredient.gameObject.SetActive(true);
        ingredient.transform.SetParent(null);
        ingredient.LaunchItSelf(transform.forward);
    }

    public bool IsBellyFull()
    {
        return belly.Count - 1 == maxIngredients;
    }
}
