using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngredientScriptable : MonoBehaviour
{
    public GameObject targetVFXGO;
    public bool istargeted;
    public string myName;
    public int baseScore;
    public Sprite myImage;
    public GameObject foodGO;
    public Rigidbody rb;
    public Collider col;
    public bool isRottenFood = false;

    public bool isSpawned = false;

    public IngredientBase ingredient;

    private void Start()
    {
        
    }

    private void Update()
    {
        if(istargeted && !targetVFXGO.activeInHierarchy){
            targetVFXGO.SetActive(true);
        }else if(!istargeted && targetVFXGO.activeInHierarchy){
            targetVFXGO.SetActive(false);
        }
    }

    public void OnCollected()
    {
        col.enabled = false;
    }

    public void LaunchItSelf(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * 20, ForceMode.Impulse);
        Destroy(gameObject, 2f);
    }

}

[Serializable]
public class IngredientBase
{
    public enum Ingredients { tomate, macarrao, queijo, camarao, pao_Hamburguer, carne, cogumelo, alho, farinha, pepperoni , rotten};
    public Ingredients ingredientEnum;
    public enum Difficulty { easy, normal, hard }
    public Difficulty difficulty;

    public IngredientScriptable ingredientScriptable;
}
