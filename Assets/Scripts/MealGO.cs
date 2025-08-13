using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MealGO : MonoBehaviour
{
    public string myName;
    public int baseScore;
    public Sprite myImage;
    public Rigidbody rb;
    public Collider col;

    [FormerlySerializedAs("meal")] public MealSo mealSo;

    private void Start()
    {
        myName = mealSo.mealName;
        baseScore = mealSo.score;
        myImage = mealSo.image;
    }

    public void LaunchItSelf(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * 20, ForceMode.Impulse);
        Destroy(gameObject, 2f);
    }
}
