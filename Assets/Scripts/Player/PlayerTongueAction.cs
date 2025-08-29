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
    
    [Header("Events")] 
    [SerializeField] private EventChannelTutorialAction eventChannelTutorialAction;
    
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
        
        eventChannelTutorialAction.RaiseEvent(TutorialAction.LaunchTongue);

    }

    IEnumerator TongueVisibleTimer()
    {
        yield return new WaitForSeconds(timeTongueShowing);
        //tongue.SetActive(false);
        OnRequestStateChage?.Invoke(CharState.Idle);
    }

}
