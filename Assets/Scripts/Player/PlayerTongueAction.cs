using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
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
    
    [FormerlySerializedAs("eventChannelTutorialAction")]
    [Header("Events")] 
    [SerializeField] private EventChannelAction eventChannelAction;
    
    public void LaunchTongue()
    {
        if (Time.time < tongueTimer || GameManager.instance.gameStates is not GameStates.game)
            return;
        
        int rand = Random.Range(0, tongueClip.Length);
        audioSource.PlayOneShot(tongueClip[rand]);
        frog_animation_controller.am.SetTrigger("Attack");
        tongueTimer = 0;
        OnRequestStateChage?.Invoke(CharState.UsingTongue);
        StartCoroutine(TongueVisibleTimer());

        tongueTimer = Time.time + tongueCooldown;
        
        eventChannelAction.RaiseEvent(new InGameAction(GameAction.LaunchTongue, 1));

    }

    IEnumerator TongueVisibleTimer()
    {
        yield return new WaitForSeconds(timeTongueShowing);
        //tongue.SetActive(false);
        OnRequestStateChage?.Invoke(CharState.Idle);
    }

}
