using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Shop : MonoBehaviour, IInteractable
{
    [Header("References")]
    public ItemDatabaseSO itemDatabase;
    public List<ItemSpawner> spawners = new List<ItemSpawner>();
    public PlayerStats playerStats;
    public ItemShopCollector itemCollector;

    // NEW: Reference to the AudioManager in the scene
    private AudioManager sceneAudio;

    [Header("Audio Settings")]
    [Tooltip("The name of the SFX clip in the AudioManager to play upon a successful purchase.")]
    public string buySoundClipName = "Purchase"; // Set this in the Inspector!

    [Header("UI")]
    public TextMeshProUGUI interactText;
    private GameObject player;

    private void Awake()
    {
        // Find the AudioManager in the scene
        sceneAudio = FindObjectOfType<AudioManager>();
        if (sceneAudio == null)
        {
            Debug.LogWarning("Shop requires an AudioManager in the scene to play sounds!");
        }

        foreach (var spawner in spawners)
        {
            // Get all items of the spawner's type
            var filteredItems = itemDatabase.GetItemsByType(spawner.spawnerItemType);

            // Remove items that have price <= 0
            filteredItems = filteredItems.FindAll(item => item.price > 0);

            spawner.shopItems = filteredItems;
        }
    }

    private void Start()
    {
        ResetShop(); // When scene loads, items appear cleanly
    }

    private void Update()
    {
        if (player && PlayerController.Instance.onInteract)
            Interact();
    }

    public void Interact()
    {
        interactText.gameObject.SetActive(false);
        TryPurchase();
    }

    private void TryPurchase()
    {
        if (playerStats.coins < itemCollector.totalPrice)
        {
            Debug.Log("Not enough coins!");
            // Optional: Play a "Purchase Failed" or "Error" SFX here if you have one.
            return;
        }

        // --- SUCCESSFUL PURCHASE LOGIC ---
        playerStats.UpdateCoins(-itemCollector.totalPrice);
        itemCollector.Purchase();

        // NEW: Play the buy sound!
        if (sceneAudio != null && !string.IsNullOrEmpty(buySoundClipName))
        {
            sceneAudio.PlaySFX(buySoundClipName);
        }
        // ----------------------------------
    }

    /// <summary>
    /// Clean re-placement of items (used when player re-enters the shop)
    /// </summary>
    public void ResetShop()
    {
        foreach (var spawner in spawners)
            spawner.ResetPedestals();
    }

    /// <summary>
    /// NEW ITEMS (costs gold)
    /// </summary>
    public void RefreshShop()
    {
        foreach (var spawner in spawners)
            spawner.RefreshNewSet();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            interactText.text = "Press E to Buy Items";
            interactText.gameObject.SetActive(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            interactText.gameObject.SetActive(false);
        }
    }
}