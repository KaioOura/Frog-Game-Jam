using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_event_controller : MonoBehaviour
{
    public Tongue tonguescript;

    public BellyFrog bellyFrog;
    
    public Vector3 offset;
    // Start is called before the first frame update

    public void TongueEvent_Chew(){
        tonguescript.OnTongueRecover();
    }

    public void LaunchRightMeal(float y){
        offset = new Vector3(0,y,0);
        bellyFrog.MoveMealToCart(offset);
    }
    
}
