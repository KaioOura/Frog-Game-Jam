using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTongueAction : MonoBehaviour
{
    public GameObject tongue;
    public Tongue tongueScript;
    public BellyFrog bellyFrog;

    public Animation_Controller frog_animation_controller;

    public float tongueCooldown;
    float tongueTimer;
    public float timeTongueShowing = 0.3f;

    public bool isUsingTongue;


    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip[] tongueClip;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates != GameStates.game)
            return;

        tongueTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Z))
        {
            LaunchTongue();
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.X))
        {
            bellyFrog.ThrowUpAllIngredients();
        }
    }

    public void LaunchTongue()
    {
        if (tongueTimer < tongueCooldown)
            return;
        
        int rand = Random.Range(0, tongueClip.Length);
        audioSource.PlayOneShot(tongueClip[rand]);
        frog_animation_controller.am.SetTrigger("Attack");
        tongueTimer = 0;
        isUsingTongue = true;
        //tongue.SetActive(true);
        tongueScript.AttackTongue();
        StartCoroutine(TongueVisibleTimer());

    }

    IEnumerator TongueVisibleTimer()
    {
        yield return new WaitForSeconds(timeTongueShowing);
        //tongue.SetActive(false);
        isUsingTongue = false;
    }

}
