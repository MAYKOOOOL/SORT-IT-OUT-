using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    // NO 'public static AudioManager instance;'

    [Header("Audio Sources")]
    public AudioSource musicSource;   // Primary BGM
    public AudioSource sfxSource;
    public AudioSource musicSource2;  // Secondary BGM (NEW)

    [Header("Audio Clips")]
    [Tooltip("All Music Clips used in the game.")]
    public List<AudioClip> musicClips;
    [Tooltip("All SFX Clips used in the game.")]
    public List<AudioClip> sfxClips;

    // --- BGM Settings (Simplified for a single scene) ---
    [Header("BGM Settings")]
    [Tooltip("The music clip name to play immediately on Start.")]
    public string initialBGMName;

    void Awake()
    {
        // ... (Awake logic)
    }

    void Start()
    {
        // Play the music specified for this scene's manager on the primary source
        if (!string.IsNullOrEmpty(initialBGMName))
        {
            PlayMusic(initialBGMName);
        }
    }

    // --- Public Audio Control Methods ---

    // 1. Primary Music Control (Unchanged)
    public void PlayMusic(string name, bool loop = true)
    {
        AudioClip clip = musicClips.Find(c => c.name == name);
        if (clip != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip named '{name}' not found in musicClips list. Playing nothing.");
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // 2. Secondary Music Control (NEW)
    public void PlayMusicSecondary(string name, bool loop = true)
    {
        // Check if the secondary source is assigned before attempting to use it
        if (musicSource2 == null)
        {
            Debug.LogError("musicSource2 is not assigned in the Inspector! Cannot play secondary music.");
            return;
        }

        AudioClip clip = musicClips.Find(c => c.name == name);
        if (clip != null)
        {
            musicSource2.clip = clip;
            musicSource2.loop = loop;
            musicSource2.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip named '{name}' not found in musicClips list for secondary source.");
        }
    }

    public void StopMusicSecondary()
    {
        if (musicSource2 != null)
        {
            musicSource2.Stop();
        }
    }


    // 3. SFX Control (Unchanged)
    public void PlaySFX(string name)
    {
        AudioClip clip = sfxClips.Find(c => c.name == name);
        if (clip != null)
            sfxSource.PlayOneShot(clip);
        else
        {
            Debug.LogWarning($"SFX clip named '{name}' not found in sfxClips list.");
        }
    }

    // 4. Volume Control (musicSource2 volume must be managed separately)
    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }

    public void SetMusicVolumeSecondary(float volume) // NEW
    {
        if (musicSource2 != null)
        {
            musicSource2.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}