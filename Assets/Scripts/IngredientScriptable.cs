using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientScriptable : MonoBehaviour
{
    public string myName;
    public int baseScore;
    public Sprite myImage;
    public GameObject foodGO;
    public Rigidbody rb;
    public Collider col;

    public int posIndex;

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
