using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation_Controller : MonoBehaviour
{
    public Animator am;
    public float rotation_value;
    private float real_rotation;

    public float LayerUpdate_speed ,update_speed, layerWeight, realayerWeight;

    public PlayerMovement playermov;

    public LayerMask ColliderLayer;
    

    // Update is called once per frame
    // void Update()
    // {
    //     
    //     return;
    //     WichSideLooking();
    //     if(realayerWeight != layerWeight){
    //
    //         layerWeight += LayerUpdate_speed;
    //         if(layerWeight > realayerWeight){
    //             layerWeight = realayerWeight;
    //         }
    //     }
    //     am.SetLayerWeight(1, layerWeight);
    //     //UpdateRightOrLeft();
    //     real_rotation = Input.GetAxis("Horizontal");
    //     //Debug.Log(real_rotation);
    //     if(real_rotation != rotation_value){
    //
    //         rotation_value += update_speed;
    //         if(rotation_value > real_rotation){
    //             rotation_value = real_rotation;
    //         }
    //     }
    //     am.SetFloat("Rotation Value",rotation_value);
    //
    // }

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

    void WichSideLooking(){
        RaycastHit hit;
        if(Physics.Raycast(transform.position,transform.TransformDirection(Vector3.forward),out hit, Mathf.Infinity, ColliderLayer)){
            if(hit.collider.gameObject.name == "FrontCollider"){
                am.SetBool("FoodOut_Front", true);
            }else{
                am.SetBool("FoodOut_Front", false);
            }
            if(hit.collider.gameObject.name == "LeftCollider"){
                am.SetBool("FoodOut_Left", true);
            }else{
                am.SetBool("FoodOut_Left", false);
            }
            if(hit.collider.gameObject.name == "BackCollider"){
                am.SetBool("FoodOut_Back", true);
            }else{
                am.SetBool("FoodOut_Back", false);
            }
            
        }
    }


}
