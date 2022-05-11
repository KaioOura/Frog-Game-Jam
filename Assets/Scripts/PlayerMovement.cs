using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {  
    }

    public void TurnRight()
    {
        transform.eulerAngles += new Vector3(0, angle, 0);
    }

    public void TurnLeft()
    {
        transform.eulerAngles -= new Vector3(0, angle, 0);
    }
}
