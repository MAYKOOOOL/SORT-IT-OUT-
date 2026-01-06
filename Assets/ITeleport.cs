using TMPro;
using UnityEngine;

public class ITeleport : MonoBehaviour, IInteractable
{
    public GameObject player;
    public TextMeshProUGUI interactText;
    public GameObject startDayPrompt;

    public Transform teleportTo;
    public Animator animator;
    public int counter = 0;
    void Start()
    {
        animator = GameObject.Find("Transition Panel").GetComponent<Animator>();
        if (teleportTo != null && teleportTo.childCount > 0)
        {
            teleportTo = teleportTo.GetChild(0);
        }
    }

    void Update()
    {
        if (player != null && PlayerController.Instance.onInteract)
        {
            Interact();
        }
    }

    public void Interact()
    {
        if (startDayPrompt != null)
        {
            startDayPrompt.SetActive(true);
        }
    }

    /// <summary>
    /// Called by the Yes button on the Start Day prompt UI.
    /// </summary>

    public void PerformTeleport()
    {
        if (player == null || teleportTo == null) return;
        if (animator != null)
        {
            animator.SetTrigger("Transition");
        }
        CharacterController controller = player.GetComponent<CharacterController>();

        if (controller != null)
        {
            controller.enabled = false;
            player.transform.position = teleportTo.position;
            controller.enabled = true;
        }

        player = null;
        interactText.gameObject.SetActive(false);

        // Optional animation

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            interactText.text = "Press E to Go Up";
            interactText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            interactText.gameObject.SetActive(false);

            if (startDayPrompt != null)
                startDayPrompt.SetActive(false);
        }
    }
}
