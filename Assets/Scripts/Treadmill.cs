using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Treadmill : MonoBehaviour
{
    public Transform[] treadMillPoints;
    [FormerlySerializedAs("positions")] public List<TreadmillSpot> spots;

    public float speed;
}
