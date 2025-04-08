using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerTongueAction : MonoBehaviour
{
    public event Action<CharState> OnRequestStateChage; 
    
    public GameObject tongue;
    public Tongue tongueScript;
    public BellyFrog bellyFrog;

    public Animation_Controller frog_animation_controller;

    public float tongueCooldown;
    float tongueTimer;
    public float timeTongueShowing = 0.3f;
    
    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip[] tongueClip;

    // Start is called before the first frame update
    void Start()
    {

    }
    
    // Update is called once per frame
    // void Update()
    // {
    //     if (GameManager.instance.gameStates != GameStates.game)
    //         return;
    //
    //     tongueTimer += Time.deltaTime;
    //
    //     if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
    //     {
    //         LaunchTongue();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X))
    //     {
    //         bellyFrog.ThrowUpAllIngredients();
    //     }
    // }

    public void LaunchTongue()
    {
        if (Time.time < tongueTimer)
            return;
        
        int rand = Random.Range(0, tongueClip.Length);
        audioSource.PlayOneShot(tongueClip[rand]);
        frog_animation_controller.am.SetTrigger("Attack");
        tongueTimer = 0;
        OnRequestStateChage?.Invoke(CharState.UsingTongue);
        StartCoroutine(TongueVisibleTimer());

        tongueTimer = Time.time + tongueCooldown;

    }

    IEnumerator TongueVisibleTimer()
    {
        yield return new WaitForSeconds(timeTongueShowing);
        //tongue.SetActive(false);
        OnRequestStateChage?.Invoke(CharState.Idle);
    }

}
