using UnityEngine;

public class ShrinkGun : MonoBehaviour
{
    public GameObject bullet;
    public Transform shootPoint;
    public PlayerStats playerStats;
    public GroundItem groundItem;

    // --- Audio Integration Fields ---
    [Header("Audio")]
    [Tooltip("The name of the SFX clip to play when the gun successfully fires.")]
    public string shootSoundClipName = "Gun_Shoot";
    [Tooltip("The name of the SFX clip to play when the gun is empty.")]
    public string emptySoundClipName = "Gun_Empty";

    private AudioManager sceneAudio;
    // --------------------------------

    void Start()
    {
        groundItem = GetComponent<GroundItem>();
        playerStats = GameObject.FindFirstObjectByType<PlayerStats>();

        // Find the local scene's AudioManager
        sceneAudio = FindAnyObjectByType<AudioManager>();
        if (sceneAudio == null)
        {
            Debug.LogWarning("ShrinkGun requires an AudioManager in the scene to play sounds!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the gun is equipped and the use button is pressed
        if (transform.parent != null && transform.parent.gameObject.activeSelf && PlayerController.Instance.onCollect)
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        if (!groundItem.purchased)
        {
            return;
        }

        if (playerStats.bullets <= 0)
        {
            // --- Empty Sound Logic ---
            if (sceneAudio != null && !string.IsNullOrEmpty(emptySoundClipName))
            {
                sceneAudio.PlaySFX(emptySoundClipName);
            }
            // -------------------------

            Announcement announcement = FindAnyObjectByType<Announcement>();
            announcement.SetAnnouncement("Not Enough Crystal!!");
            return;
        }

        // --- Successful Shot Logic ---

        // 1. Play Sound
        if (sceneAudio != null && !string.IsNullOrEmpty(shootSoundClipName))
        {
            sceneAudio.PlaySFX(shootSoundClipName);
        }

        // 2. Consume bullet and spawn projectile
        playerStats.UpdateBullets(-1);
        GameObject bulletSpawn = Instantiate(bullet, shootPoint.position, shootPoint.rotation);

        // 3. Apply force
        Rigidbody rb = bulletSpawn.GetComponent<Rigidbody>();
        rb.AddForce(shootPoint.forward * 500f);

    }
}