using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    // --- Public Configuration ---
    [Header("Ground Footstep Settings")]
    [Tooltip("The time (in seconds) between each footstep sound when walking on ground.")]
    public float timeBetweenWalkSteps = 0.45f;
    [Tooltip("The time (in seconds) between each footstep sound when running on ground.")]
    public float timeBetweenRunSteps = 0.25f;
    [Tooltip("The name of the SFX clip to play for ground movement.")]
    public string groundFootstepClipName = "Footstep";

    [Header("Water Footstep Settings")]
    [Tooltip("The time (in seconds) between each splash sound when moving in water.")]
    public float timeBetweenWaterSteps = 0.6f;
    [Tooltip("The name of the SFX clip to play for water movement (e.g., 'Water_Wade').")]
    public string waterFootstepClipName = "Water_Walk";

    // --- Private References ---
    private AudioManager sceneAudio;
    private PlayerMovement playerMovement;
    private float footstepTimer;

    // Tracks movement state from the previous frame.
    private bool wasMovingLastFrame = false;

    void Start()
    {
        // 1. Get the local scene's AudioManager
        sceneAudio = FindAnyObjectByType<AudioManager>();
        if (sceneAudio == null)
        {
            Debug.LogError("PlayerFootsteps requires an AudioManager in the scene to play sounds!");
        }

        // 2. Get the player's movement script
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerFootsteps requires a PlayerMovement component on the same object.");
        }

        // Initialize the timer to 0f for instant first step
        footstepTimer = 0f;
    }

    void Update()
    {
        bool isMovingGround = playerMovement != null && playerMovement.IsWalkingOnGround();
        bool isMovingWater = playerMovement != null && playerMovement.IsWalkingInWater();

        bool isMoving = isMovingGround || isMovingWater;

        // Determine the correct interval and clip name
        float currentStepInterval;
        string currentClipName;

        if (isMovingWater)
        {
            currentStepInterval = timeBetweenWaterSteps;
            currentClipName = waterFootstepClipName;
        }
        else if (isMovingGround)
        {
            currentClipName = groundFootstepClipName;
            currentStepInterval = playerMovement.IsRunning() ? timeBetweenRunSteps : timeBetweenWalkSteps;
        }
        else
        {
            // If not moving (ground or water), default to walking interval for reset
            currentClipName = groundFootstepClipName;
            currentStepInterval = timeBetweenWalkSteps;
        }

        if (isMoving)
        {
            // 1. Check for instant first step (Transition from stopped to moving)
            // This is triggered if we were stopped last frame OR if the interval changed drastically 
            // (e.g., switching from water to ground while running) and the timer is less than the new interval.
            if (!wasMovingLastFrame || footstepTimer <= 0f)
            {
                // Play the sound immediately.
                if (sceneAudio != null)
                {
                    sceneAudio.PlaySFX(currentClipName);
                }

                // Set the timer to the full interval to start the countdown for the *second* step.
                footstepTimer = currentStepInterval;
            }
            // 2. Continuous steps (Already moving)
            else
            {
                footstepTimer -= Time.deltaTime;

                if (footstepTimer <= 0)
                {
                    if (sceneAudio != null)
                    {
                        sceneAudio.PlaySFX(currentClipName);
                    }

                    // Reset the timer for the next step
                    footstepTimer = currentStepInterval;
                }
            }
        }
        else
        {
            // Player is stopped. Reset the timer to 0f, so the next step is ready to fire instantly.
            footstepTimer = 0f;
        }

        // Update the state for the next frame's check
        wasMovingLastFrame = isMoving;
    }
}