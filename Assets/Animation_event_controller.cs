using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_event_controller : MonoBehaviour
{
    public Tongue tonguescript;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TongueEvent_Chew(){
        tonguescript.OnTongueRecover();
    }
    
}
