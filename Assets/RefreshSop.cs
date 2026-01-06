using TMPro;
using Unity.Multiplayer.Center.Common.Analytics;
using UnityEngine;

public class RefreshSop : MonoBehaviour, IInteractable
{
    public Shop shop;
    public TextMeshProUGUI interactText;
    private GameObject player;

    private const int refreshCost = 10;

    private void Update()
    {
        if (player != null && PlayerController.Instance.onInteract)
            Interact();
    }

    public void Interact()
    {
        if (shop.playerStats.coins < refreshCost) return;

        shop.playerStats.UpdateCoins(-refreshCost);
        shop.RefreshShop();

        interactText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = other.gameObject;
        interactText.text = $"Press E to Refresh Shop ({refreshCost}G)";
        interactText.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        player = null;
        interactText.gameObject.SetActive(false);
    }
}
