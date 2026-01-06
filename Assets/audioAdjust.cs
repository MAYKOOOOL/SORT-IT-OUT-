using UnityEngine;
using UnityEngine.UI;

public class audioAdjust : MonoBehaviour
{
    public AudioSource bgmSource;    // Drag your BGM AudioSource
    public AudioSource sfxSource;    // Drag your SFX AudioSource
    public Slider bgmSlider;         // Drag your BGM Slider
    public Slider sfxSlider;         // Drag your SFX Slider

    void Start()
    {
        // Initialize sliders with current volume
        bgmSlider.value = bgmSource.volume;
        sfxSlider.value = sfxSource.volume;

        // Add listeners for real-time volume adjustment
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        sfxSlider.onValueChanged.AddListener(ChangeSFXVolume);
    }

    void ChangeBGMVolume(float value)
    {
        bgmSource.volume = value;  // Adjust BGM volume
    }

    void ChangeSFXVolume(float value)
    {
        sfxSource.volume = value;  // Adjust SFX volume
    }
}
