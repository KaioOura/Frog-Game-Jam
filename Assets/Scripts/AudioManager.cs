using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using DG.Tweening;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioMixer audioMixer;

    public AudioSource audioSource;
    public AudioClip[] gameMusics;
    public AudioClip menuGame;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

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
        audioMixer.SetFloat("VolumeMaster", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMasterVolume));
        audioMixer.SetFloat("VolumeMusic", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMusicVolume));
        audioMixer.SetFloat("VolumeSFX", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioSFXVolume));

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

    public void Mute()
    {
        float volume;
        audioMixer.GetFloat("VolumeMaster", out volume);

        if (volume == -80)
        {
            audioMixer.SetFloat("VolumeMaster", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMasterVolume));
        }
        else
        {
            audioMixer.SetFloat("VolumeMaster", -80);
        }
    }

    public void OnMasterValueChanged(float value)
    {
        audioMixer.SetFloat("VolumeMaster", value);
        PlayerPrefs.SetFloat(PlayerPrefsSettings.audioMasterVolume, value);
    }

    public void OnMusicValueChanged(float value)
    {
        audioMixer.SetFloat("VolumeMusic", value);
        PlayerPrefs.SetFloat(PlayerPrefsSettings.audioMusicVolume, value);
    }

    public void OnSFXValueChanged(float value)
    {
        audioMixer.SetFloat("VolumeSFX", value);
        PlayerPrefs.SetFloat(PlayerPrefsSettings.audioSFXVolume, value);
    }
}
