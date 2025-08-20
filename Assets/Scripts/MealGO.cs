using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MealGO : MonoBehaviour
{
    public Rigidbody rb;
    public Collider col;

    [FormerlySerializedAs("meal")] public MealSo mealSo;

    private void Start()
    {

    }

    public void LaunchItSelf(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * 20, ForceMode.Impulse);
        Destroy(gameObject, 2f);
    }
}
