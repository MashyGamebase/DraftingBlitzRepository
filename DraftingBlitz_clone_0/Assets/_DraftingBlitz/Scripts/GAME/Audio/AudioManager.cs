using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer audioMixer;

    [Header("Mixer Parameters")]
    [Range(0.0001f, 1f)] public float bgmVolume = 1f;
    [Range(0.0001f, 1f)] public float sfxVolume = 1f;

    private const string BGM_PARAM = "BGMVolume";
    private const string SFX_PARAM = "SFXVolume";

    private bool isBgmOn = true;
    private bool isSfxOn = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(BGM_PARAM, db);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        float db = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(SFX_PARAM, db);
    }

    public void ToggleBGM(bool toggle)
    {
        isBgmOn = !isBgmOn;
        SetBGMVolume(toggle ? 0.6f : 0.0001f); // -80dB effectively mutes
    }

    public void ToggleSFX(bool toggle)
    {
        isSfxOn = !isSfxOn;
        SetSFXVolume(toggle ? 1 : 0.0001f);
    }

    // Optional: Save/Load Preferences (PlayerPrefs)
    public void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("BGMVolume", bgmVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void LoadVolumeSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat("BGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ApplyVolumes();
    }
}
