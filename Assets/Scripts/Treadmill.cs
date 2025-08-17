using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Treadmill : MonoBehaviour
{
    public Transform[] points;
    [FormerlySerializedAs("positions")] public List<FoodPlate> plates;
    [SerializeField] private float spacing;

    public float speed;

    public void Start()
    {
        for (int i = 0; i < plates.Count; i++)
        {
            plates[i].transform.localPosition = new Vector3(plates[i].transform.localPosition.x,plates[i].transform.localPosition.y, i * spacing);
        }
    }
}
