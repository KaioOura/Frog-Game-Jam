using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTongueAction : MonoBehaviour
{
    public GameObject tongue;

    public float tongueCooldown;
    float tongueTimer;
    public float timeTongueShowing = 0.3f;

    public bool isUsingTongue;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip tongueClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tongueTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.W) && tongueTimer > tongueCooldown)
        {
            LaunchTongue();
        }
    }

    public void LaunchTongue()
    {
        //audioSource.PlayOneShot(tongueClip);

        tongueTimer = 0;
        isUsingTongue = true;
        tongue.SetActive(true);
        StartCoroutine(TongueVisibleTimer());
        
    }

    IEnumerator TongueVisibleTimer()
    {
        yield return new WaitForSeconds(timeTongueShowing);
        tongue.SetActive(false);
        isUsingTongue = false;
    }

}
