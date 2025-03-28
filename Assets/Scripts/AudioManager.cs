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

    public float audioSourceMaxVolume;

    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderVolume;

    public float musicLength;
    public AudioClip lastMusic;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSourceMaxVolume = audioSource.volume;

        PlayMenuMusic();
        audioMixer.SetFloat("VolumeMaster", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMasterVolume, -10));
        audioMixer.SetFloat("VolumeMusic", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMusicVolume, -10));
        audioMixer.SetFloat("VolumeSFX", PlayerPrefs.GetFloat(PlayerPrefsSettings.audioSFXVolume, -10));

        sliderMaster.value = PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMasterVolume, -10);
        sliderMusic.value = PlayerPrefs.GetFloat(PlayerPrefsSettings.audioMusicVolume, -10);
        sliderVolume.value = PlayerPrefs.GetFloat(PlayerPrefsSettings.audioSFXVolume, -10);

    }
    
    public void PlayMenuMusic()
    {
        ChangeMusic(menuGame);
    }

    public void PlayGameMusic()
    {
        int rand = Random.Range(0, gameMusics.Length);
        ChangeMusic(gameMusics[rand], playMuiscAfterEnd: true);
    }

    public void ChangeMusic(AudioClip audioClip, bool playMuiscAfterEnd = false)
    {
        lastMusic = audioClip;

        float to = 0;
        DOTween.To(() => audioSource.volume, x => audioSource.volume = x, to, 0.5f).OnComplete(() =>
        {
            audioSource.Stop();
            audioSource.loop = true;
            audioSource.clip = audioClip;
            audioSource.Play();

            musicLength = audioClip.length;

            DOTween.To(() => audioSource.volume, x => audioSource.volume = x, audioSourceMaxVolume, 1);

            if (playMuiscAfterEnd)
                StartCoroutine(PlayMusicDelayed());

        });
    }

    IEnumerator PlayMusicDelayed()
    {
        yield return new WaitForSeconds(musicLength);
        int rand = Random.Range(0, gameMusics.Length);

        while (lastMusic == gameMusics[rand])
        {
            rand = Random.Range(0, gameMusics.Length);
            yield return null;
        }

        ChangeMusic(gameMusics[rand], playMuiscAfterEnd: true);
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
