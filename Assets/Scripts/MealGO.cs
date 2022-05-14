using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MealGO : MonoBehaviour
{
    public string myName;
    public int baseScore;
    public Sprite myImage;
    public Rigidbody rb;
    public Collider col;

    public Meal meal;

    private void Start()
    {
        myName = meal.mealName;
        baseScore = meal.score;
        myImage = meal.image;
    }

    public void LaunchItSelf(Vector3 dir)
    {
        rb.isKinematic = false;
        rb.AddForce(dir * 20, ForceMode.Impulse);
        Destroy(gameObject, 2f);
    }
}
