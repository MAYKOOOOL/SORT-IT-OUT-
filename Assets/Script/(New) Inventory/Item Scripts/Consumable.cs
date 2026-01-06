using UnityEngine;

public class Consumable : MonoBehaviour
{
    // --- Audio Integration Field ---
    private AudioManager sceneAudio;
    public string obtainSoundClipName = "obtain";
    // -------------------------------

    protected PlayerStats playerStats;
    protected GroundItem groundItem;
    public ParticleSystem useEffect;

    protected virtual void Start()
    {
        // Find the local scene's AudioManager
        sceneAudio = FindAnyObjectByType<AudioManager>();
        if (sceneAudio == null)
        {
            Debug.LogWarning("Consumable requires an AudioManager in the scene to play sounds!");
        }

        groundItem = GetComponent<GroundItem>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerStats = player.GetComponent<PlayerStats>();
    }

    protected virtual void Update()
    {
        // Only use item if parent exists and active, player pressed use, and item purchased
        if (transform.parent != null && transform.parent.gameObject.activeSelf &&
            PlayerController.Instance.onUse && groundItem != null && groundItem.purchased)
        {
            UseItem();
        }
    }

    public virtual void UseItem()
    {
        // 1. Play the 'obtain' sound effect
        if (sceneAudio != null && !string.IsNullOrEmpty(obtainSoundClipName))
        {
            sceneAudio.PlaySFX(obtainSoundClipName);
        }

        // 2. Play the particle effect
        Instantiate(useEffect, transform.position, Quaternion.identity);

        // 3. (Override in child classes to apply health/energy/etc. effect)
    }
}