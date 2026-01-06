using UnityEngine;

public class Animation : MonoBehaviour
{
    public PlayerController player;
    public Animator animator;
    public ParticleSystem dustParticle;
    public bool hasItem = false;
    public bool hasEquipment = false;
    private Color originalDustColor;
    void Start()
    {
        originalDustColor = dustParticle.main.startColor.color;
        animator = GetComponent<Animator>();
        player = PlayerController.Instance;

    }

    void Update()
    {
        bool isMoving = player.onMove;
        bool isRunning = player.onRun;
        bool isJump = player.onJump;
        // GENERAL ITEM (any item in slot)
        hasItem = player.hasItemInSlot;
        animator.SetBool("HasItem", hasItem);

        // SPECIFIC EQUIPMENT (item ID 1)
        hasEquipment = player.onAim;
        animator.SetBool("HasEquipment", hasEquipment);

        // HAND SWINGING (only if NOT holding equipment)
        if (!hasEquipment && isMoving)
        {
            animator.SetBool("MoveHand", true);
        }
        else
        {
            animator.SetBool("MoveHand", false);
        }

        // FEET MOVEMENT
        animator.SetBool("MoveFeet", isMoving);

        // RUNNING = faster hand animation
        if (isRunning && isMoving)
            animator.speed = 2f;   // double speed
        else
            animator.speed = 1f;   // normal

        // DUST PARTICLE when moving
        if (isMoving && !dustParticle.isPlaying && !isJump)
        {
            dustParticle.Play();
        }
        if (!isMoving && dustParticle.isPlaying)
        {
            dustParticle.Stop();
        }

        var main = dustParticle.main;

        if (player.onSwim)
        {
            main.startColor = new Color(0.35f, 0.45f, 0.2f, 1f);
        }
        else
        {
            main.startColor = originalDustColor; // Back to original
        }
    }
}
