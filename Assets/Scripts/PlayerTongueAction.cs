using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTongueAction : MonoBehaviour
{
    public GameObject tongue;

    public float tongueCooldown;
    float tongueTimer;

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
        tongueTimer = 0;
        tongue.SetActive(true);
        StartCoroutine(TongueVisibleTimer());
        
    }

    IEnumerator TongueVisibleTimer()
    {
        yield return new WaitForSeconds(0.5f);
        tongue.SetActive(false);
    }



}
