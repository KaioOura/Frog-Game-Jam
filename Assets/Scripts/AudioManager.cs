using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip[] gameMusics;
    public AudioClip menuGame;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStates == GameManager.GameStates.game)
        {
            PlayGameMusic();
        }
    }

    public void PlayMenuMusic()
    {
        audioSource.Pause();
        audioSource.loop = true;
        audioSource.clip = menuGame;
        audioSource.Play();
    }

    public void PlayGameMusic()
    {
        if (!audioSource.isPlaying)
        {
            int rand = Random.Range(0, gameMusics.Length);

            audioSource.loop = true;
            audioSource.clip = gameMusics[rand];
            audioSource.Play();

        }
    }
}
