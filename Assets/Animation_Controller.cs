using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    public Animator am;
    public float rotation_value;
    private float real_rotation;

    public float update_speed;

    public PlayerMovement playermov;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateRightOrLeft();
        real_rotation = Input.GetAxis("Horizontal");
        Debug.Log(real_rotation);
        if(real_rotation != rotation_value){

            rotation_value += update_speed;
            if(rotation_value > real_rotation){
                rotation_value = real_rotation;
            }
        }
        am.SetFloat("Rotation Value",rotation_value);

    }

    void UpdateRightOrLeft(){
        if(!Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.D) && rotation_value!=0){
            am.SetBool("Input Pressed", true);
            am.SetBool("FT", false);

        }else{
            if(am.GetBool("Input Pressed")){
                am.SetBool("FT", true);
                StartCoroutine(ResetFT());
            }
            am.SetBool("Input Pressed", false);

        }

        /*if(Mathf.Abs(real_rotation) > 0){
            am.SetBool("FT", true);

        }else if(real_rotation==0 && am.GetBool("Input Pressed")){
            am.SetBool("FT", true);
        }*/
        if(rotation_value > 0){
            am.SetBool("Right", true);
        }else{
            am.SetBool("Right", false);
        }
    }

    IEnumerator ResetFT(){
        yield return new WaitForSeconds(0.2f);
        if(am.GetBool("FT")){
            am.SetBool("FT", false);
        }
    }
}
