using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;


public class Tongue : MonoBehaviour
{
    public BellyFrog bellyFrog;
    public Animator an;
    public Transform ingredientPos;
    public bool isTongueOccupied;
    public IngredientScriptable ingredientCollected;

    public AudioSource audioSource;
    public AudioClip[] swallow;
    
    public void OnTriggerEnter(Collider other)
    {
        Profiler.BeginSample("Kaio Profiller: Picked Ingredient");
        if (other.CompareTag("Pickable") && !isTongueOccupied && !bellyFrog.IsBellyFull())
        {
            if (other.TryGetComponent(out IngredientScriptable ingredient))
            {
                OnTongueHit(ingredient);
                Debug.Log("Licked: " + other.gameObject);
            }
            
        }
        Profiler.EndSample();
    }

    public void OnTongueHit(IngredientScriptable ingredient)
    {
        isTongueOccupied = true;
        ingredientCollected = ingredient;
        ingredientCollected.OnCollected();
        ingredientCollected.transform.SetParent(ingredientPos);
        ingredientCollected.transform.localPosition = Vector3.zero;
    }

    public void OnTongueRecover()
    {
        isTongueOccupied = false;
        //Adicionar ingrediente na lista de ingredientes na barriga
        if (ingredientCollected != null)
        {
            int rand = Random.Range(0, swallow.Length);
            audioSource.PlayOneShot(swallow[rand]);
            bellyFrog.AddIngredient(ingredientCollected);
            ingredientCollected = null;
        }

    }
}
