using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class Ingredient : MonoBehaviour
{
    public GameObject targetVFXGO;
    public bool istargeted;
   

    public GameObject foodGO;
    public Rigidbody rb;
    public Collider col;
    public bool isSpawned = false;

    [FormerlySerializedAs("infIngredientSo")] public IngredientSo IngredientSo;

    private void Start()
    {
        
    }

    private void Update()
    {
        // if(istargeted && !targetVFXGO.activeInHierarchy){
        //     targetVFXGO.SetActive(true);
        // }else if(!istargeted && targetVFXGO.activeInHierarchy){
        //     targetVFXGO.SetActive(false);
        // }
    }

    public void UpdateTargetVFXGO(bool shouldActivate)
    {
        targetVFXGO.SetActive(shouldActivate);
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
   
  
 

   
}
