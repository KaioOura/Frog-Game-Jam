using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class Ingredient : MonoBehaviour, IInteractable
{
    public Action<Ingredient> OnReleaseToPool;
    public Action OnGetRemovedFromPlate;

    public GameObject targetVFXGO;
    public Rigidbody rb;
    public Collider col;

    [FormerlySerializedAs("infIngredientSo")]
    public IngredientSo IngredientSo;

    public void OnCollected()
    {
        col.enabled = false;
        OnGetRemovedFromPlate?.Invoke();
    }

    public void LaunchItSelf(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * 20, ForceMode.Impulse);
        StartCoroutine(ReleaseToPoolRoutine());
    }

    IEnumerator ReleaseToPoolRoutine()
    {
        yield return new WaitForSeconds(2f);
        ReleaseToPool();
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

    public void ReleaseToPool()
    {
        OnDeselected();
        OnGetRemovedFromPlate = null;
        col.enabled = true;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
        OnReleaseToPool(this);
    }
}