using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

public class Teleport : MonoBehaviour
{
    public Transform teleportTo;
    public Animator animator;
    private GameObject playerToTeleport;
    private bool isPaused = false;
    private float pauseEndTime = 0f;
    private bool isPlayingAnimation = false;
    private void Start()
    {
        animator = GameObject.Find("Transition Panel").GetComponent<Animator>();

        if (teleportTo != null && teleportTo.childCount > 0)
        {
            teleportTo = teleportTo.GetChild(0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isPaused || teleportTo == null) return;

        if (other.CompareTag("Player"))
        {
            playerToTeleport = other.gameObject;
            animator.SetTrigger("Transition");
            Invoke(nameof(DelayedTeleport), 0.2f);
        }
    }

    public void DelayedTeleportPlayer()
    {
        playerToTeleport = FindAnyObjectByType<PlayerMovement>().gameObject;
        animator.SetTrigger("Transition");
        Invoke(nameof(DelayedTeleport), 0.2f);
    }

    public void TeleportPlayer(GameObject player)
    {
        
        if (teleportTo == null || player == null) return;
        
        CharacterController controller = player.GetComponent<CharacterController>();
        Trash trashObject = player.GetComponent<Trash>();
        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = teleportTo.position;
            controller.enabled = true;
        }
        if(trashObject != null)
        {
            trashObject.gameObject.transform.position = teleportTo.position;
        }

        //Optionally trigger animation
        
    }
    private void DelayedTeleport()
    {
        if (playerToTeleport != null)
        {
            TeleportPlayer(playerToTeleport);
            playerToTeleport = null;
        }
    }

    private void Update()
    {

    }
}
