using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class Ingredient : MonoBehaviour, IInteractable
{
    public GameObject targetVFXGO;
    public Rigidbody rb;
    public Collider col;

    [FormerlySerializedAs("infIngredientSo")] public IngredientSo IngredientSo;
    
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

    public void OnInteract()
    {
        throw new NotImplementedException();
    }

    public void OnSelected()
    {
        targetVFXGO.SetActive(true);
    }

    public void OnDeselected()
    {
        targetVFXGO.SetActive(false);
    }

    public void OnInteractEnded()
    {
        throw new NotImplementedException();
    }
}

[Serializable]
public class IngredientBase
{
   
  
 

   
}
