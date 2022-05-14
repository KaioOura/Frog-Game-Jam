using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IngredientScriptable : MonoBehaviour
{
    public string myName;
    public int baseScore;
    public Sprite myImage;
    public GameObject foodGO;
    public Rigidbody rb;
    public Collider col;

    public IngredientBase ingredient;

    private void Start()
    {
        
    }

    private void Update()
    {
        
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
    public enum Ingredients { tomate, macarrao, queijo, camarao, pao_Hamburguer, carne, cogumelo };
    public Ingredients ingredientEnum;
    public IngredientScriptable ingredientScriptable;
}
