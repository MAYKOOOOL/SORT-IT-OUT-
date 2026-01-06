using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Dropdown videoDropdown;

    [Header("Audio Buttons")]
    public Button mainVolumeButton;
    public Button musicButton;
    public Button sfxButton;

    [Header("Audio Sliders")]
    public Slider mainVolumeSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource[] sfxSources;

    private float lastMainVolume = 1f;
    private float lastMusicVolume = 1f;
    private float lastSFXVolume = 1f;

    void Start()
    {
        videoDropdown.ClearOptions();
        videoDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Fullscreen",
            "Windowed",
            "1920 x 1080",
            "1024 x 768"
        });

        if (Screen.fullScreen)
            videoDropdown.value = 0;
        else
            videoDropdown.value = 1;

        videoDropdown.onValueChanged.AddListener(SetVideoOption);

        mainVolumeButton.onClick.AddListener(ToggleMainVolume);
        musicButton.onClick.AddListener(ToggleMusic);
        sfxButton.onClick.AddListener(ToggleSFX);

        mainVolumeSlider.onValueChanged.AddListener(SetMainVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);

        mainVolumeSlider.value = AudioListener.volume;
        if (musicSource != null) musicSlider.value = musicSource.volume;
        if (sfxSources.Length > 0) sfxSlider.value = sfxSources[0].volume;
    }

    void SetVideoOption(int index)
    {
        switch (index)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 2:
                Screen.SetResolution(1920, 1080, Screen.fullScreen);
                break;
            case 3:
                Screen.SetResolution(1024, 768, Screen.fullScreen);
                break;
        }
    }

    void ToggleMainVolume()
    {
        if (AudioListener.volume > 0)
        {
            lastMainVolume = AudioListener.volume;
            mainVolumeSlider.value = 0;
            AudioListener.volume = 0;
        }
        else
        {
            mainVolumeSlider.value = lastMainVolume;
            AudioListener.volume = lastMainVolume;
        }
    }

    void ToggleMusic()
    {
        if (musicSource == null) return;

        if (musicSource.volume > 0)
        {
            lastMusicVolume = musicSource.volume;
            musicSlider.value = 0;
            musicSource.volume = 0;
        }
        else
        {
            musicSlider.value = lastMusicVolume;
            musicSource.volume = lastMusicVolume;
        }
    }

    void ToggleSFX()
    {
        if (sfxSources.Length == 0) return;

        if (sfxSources[0].volume > 0)
        {
            lastSFXVolume = sfxSources[0].volume;
            sfxSlider.value = 0;
            foreach (var sfx in sfxSources)
                sfx.volume = 0;
        }
        else
        {
            sfxSlider.value = lastSFXVolume;
            foreach (var sfx in sfxSources)
                sfx.volume = lastSFXVolume;
        }
    }

    void SetMainVolume(float value)
    {
        AudioListener.volume = value;
        if (value > 0) lastMainVolume = value;
    }

    void SetMusicVolume(float value)
    {
        if (musicSource == null) return;
        musicSource.volume = value;
        if (value > 0) lastMusicVolume = value;
    }

    void SetSFXVolume(float value)
    {
        foreach (var sfx in sfxSources)
        {
            if (sfx != null) sfx.volume = value;
        }
        if (value > 0) lastSFXVolume = value;
    }
}
