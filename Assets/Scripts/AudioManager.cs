using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer audioMixer;

    public AudioSource audioSource;
    public AudioClip[] gameMusics;
    public AudioClip menuGame;

    public float audioSourceMaxVolume;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSourceMaxVolume = audioSource.volume;

        PlayMenuMusic();
    }

    // Update is called once per frame
    void Update()
    {
        //if (GameManager.instance.gameStates == GameManager.GameStates.game)
        //{
        //    PlayGameMusic();
        //}
    }

    public void PlayMenuMusic()
    {
        ChangeMusic(menuGame);
    }

    public void PlayGameMusic()
    {
        int rand = Random.Range(0, gameMusics.Length);
        ChangeMusic(gameMusics[rand]);
    }

    public void ChangeMusic(AudioClip audioClip)
    {
        float to = 0;
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, to, 0.5f).OnComplete(() =>
        {
            audioSource.Stop();
            audioSource.loop = true;
            audioSource.clip = audioClip;
            audioSource.Play();


            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, audioSourceMaxVolume, 1);

        });
    }
}
