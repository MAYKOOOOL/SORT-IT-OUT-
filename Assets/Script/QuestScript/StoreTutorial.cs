using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class StoreTutorial : MonoBehaviour
{
    public enum StoreState
    {
        SearchingForBag, // 1. Brute force refresh until bag appears
        WaitForPlayer,   // 2. Bag found, waiting for trigger touch
        CollectCoin,     // 3. Tutorial Starts
        UseCoin,
        PickupBag,
        PlaceOnCounter,
        BuyItem,
        RetrieveBag,
        UseBag,
        Finished
    }

    public ExploreTutorial exploreTutorial;

    [Header("Setup")]
    public GameObject coinPrefab;
    public Transform coinSpawnPoint;
    public int bagItemID = 36;
    public int coinItemID = 35;

    [Header("Shop References")]
    public Shop shopScript; // REQUIRED: Reference to the Shop script to call Refresh()
    public ItemShopCollector counterCollector;

    [Header("References")]
    public QuestUI questUI;
    public ArrowPointer arrowPointer;
    public PlayerController playerController;
    public InventorySO playerInventory;

    // --- Internal State ---
    public StoreState currentState = StoreState.SearchingForBag;

    private GameObject activeCoin;
    private GameObject activeBag;
    private GroundItem bagGroundItem;

    // --- CONSUMPTION FLAGS ---
    private bool coinWasConsumed = false;
    private bool bagWasConsumed = false;

    // Timer
    private float refreshTimer = 0f;
    private float refreshDelay = 0.1f;

    void Start()
    {
        if (questUI == null) questUI = FindAnyObjectByType<QuestUI>();
        if (arrowPointer == null) arrowPointer = FindAnyObjectByType<ArrowPointer>();
        if (playerController == null) playerController = PlayerController.Instance;
        if (playerInventory == null && playerController != null) playerInventory = playerController.inventory;
        if (shopScript == null) shopScript = FindAnyObjectByType<Shop>();

        // Spawn Coin immediately so it's ready when player arrives
        if (coinPrefab != null && coinSpawnPoint != null)
        {
            activeCoin = Instantiate(coinPrefab, coinSpawnPoint.position, Quaternion.identity);
        }
    }

    void Update()
    {
        // 1. PHASE 1: Find the Bag (Runs before player arrives)
        if (currentState == StoreState.SearchingForBag)
        {
            HandleBagSearch();
            return;
        }

        // 2. PHASE 2: Wait for Player (Bag is ready)
        if (currentState == StoreState.WaitForPlayer)
        {
            return; // Waiting for OnTriggerEnter
        }

        // 3. PHASE 3: The Tutorial (Player has arrived)
        if (currentState == StoreState.Finished) return;

        switch (currentState)
        {
            case StoreState.CollectCoin:
                HandleCollectCoin();
                break;
            case StoreState.UseCoin:
                HandleUseCoin();
                break;
            case StoreState.PickupBag:
                HandlePickupBag();
                break;
            case StoreState.PlaceOnCounter:
                HandlePlaceOnCounter();
                break;
            case StoreState.BuyItem:
                HandleBuyItem();
                break;
            case StoreState.RetrieveBag:
                HandleRetrieveBag();
                break;
            case StoreState.UseBag:
                HandleUseBag();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // If we found the bag and are waiting -> Start Tutorial
            if (currentState == StoreState.WaitForPlayer)
            {
                BeginTutorial();
            }
            // If finished -> Cleanup
            else if (currentState == StoreState.Finished)
            {
                questUI.Clear();
                exploreTutorial.BeginTutorial();
                Destroy(this);
            }
        }
    }

    void BeginTutorial()
    {
        // Transition to first gameplay state
        ChangeState(StoreState.CollectCoin);

        if (questUI != null) questUI.OpenQuest();
        Debug.Log("Store Tutorial Started!");
    }

    // ---------------------------------------------------------
    // EVENT RECEIVERS (Called by Coin.cs and Bag.cs)
    // ---------------------------------------------------------
    public void ReportCoinConsumed()
    {
        coinWasConsumed = true;
        // If waiting for use, move to Pickup Bag
        if (currentState == StoreState.UseCoin)
        {
            ChangeState(StoreState.PickupBag);
        }
    }

    public void ReportBagConsumed()
    {
        bagWasConsumed = true;
        // If waiting for usage, finish
        if (currentState == StoreState.UseBag)
        {
            FinishTutorial();
        }
    }

    // ---------------------------------------------------------
    // STATE LOGIC
    // ---------------------------------------------------------

    void HandleBagSearch()
    {
        refreshTimer -= Time.deltaTime;
        if (refreshTimer > 0) return;
        refreshTimer = refreshDelay;

        // 1. Look for existing Bag in scene
        GroundItem foundBag = FindBagInScene();

        if (foundBag != null)
        {
            // FOUND IT! Reference it immediately
            activeBag = foundBag.gameObject;
            bagGroundItem = foundBag;

            Debug.Log("Bag (ID 36) Found in Shop! Reference Stored.");
            currentState = StoreState.WaitForPlayer; // Stop searching, wait for player
        }
        else
        {
            // NOT FOUND. Force the shop to reroll items.
            if (shopScript != null)
            {
                shopScript.RefreshShop();
            }
        }
    }

    GroundItem FindBagInScene()
    {
        GroundItem[] allItems = FindObjectsByType<GroundItem>(FindObjectsSortMode.None);
        foreach (GroundItem item in allItems)
        {
            if (item.parentPedestal != null && item.parentPedestal.itemData != null)
            {
                if (item.parentPedestal.itemData.id == bagItemID)
                {
                    return item;
                }
            }
        }
        return null;
    }

    // 1. Collect Coin
    void HandleCollectCoin()
    {
        if (activeCoin != null) arrowPointer.SetTarget(activeCoin.transform);
        else arrowPointer.DisableArrow();

        questUI.UpdateDisplay(" Get Money", "Collect the Coin on the floor (Press E).");

        if (HasItemInInventory(coinItemID))
        {
            ChangeState(StoreState.UseCoin);
        }
    }

    // 2. Use Coin
    void HandleUseCoin()
    {
        arrowPointer.DisableArrow();
        questUI.UpdateDisplay(" Get Money", "Select the Coin in inventory and press <b>Use (Left Click)</b> to get Gold.");

        // Wait for event or fallback
        if (coinWasConsumed)
        {
            ChangeState(StoreState.PickupBag);
        }
    }

    // 3. Pickup Bag
    void HandlePickupBag()
    {
        // 1. Ensure we have a valid reference to point at
        // If activeBag is null or destroyed (but not in inventory yet), try to find it again
        if (activeBag == null)
        {
            GroundItem found = FindBagInScene();
            if (found != null)
            {
                activeBag = found.gameObject;
                bagGroundItem = found;
            }
        }

        // 2. Point Arrow
        if (activeBag != null)
        {
            arrowPointer.SetTarget(activeBag.transform);
            questUI.UpdateDisplay(" Shopping", "Go to the Pedestal and collect the <b>Bag</b>.");
        }
        else
        {
            // Fallback if bag is lost but player has it?
            if (HasItemInInventory(bagItemID))
            {
                ChangeState(StoreState.PlaceOnCounter);
                return;
            }
            arrowPointer.DisableArrow();
        }

        // 3. Check Logic: Did player pick it up?
        if (HasItemInInventory(bagItemID))
        {
            ChangeState(StoreState.PlaceOnCounter);
        }
    }

    // 4. Place on Counter
    void HandlePlaceOnCounter()
    {
        arrowPointer.SetTarget(counterCollector.transform);
        questUI.UpdateDisplay(" Checkout", "Select the Bag slot and press <b>[Q]</b> to place it on the Counter.");

        GameObject bagOnCounter = FindBagOnCounter();
        if (bagOnCounter != null)
        {
            // Update reference to the physical object on counter
            activeBag = bagOnCounter;
            bagGroundItem = activeBag.GetComponent<GroundItem>();
            ChangeState(StoreState.BuyItem);
        }
    }

    // 5. Buy Item
    void HandleBuyItem()
    {
        arrowPointer.SetTarget(counterCollector.transform);
        questUI.UpdateDisplay(" Purchase", "Interact with [E] in the Shop Counter to buy the item.");

        if (bagGroundItem != null && bagGroundItem.purchased)
        {
            ChangeState(StoreState.RetrieveBag);
        }
        // Fallback: If removed from counter without buying
        else if (FindBagOnCounter() == null)
        {
            ChangeState(StoreState.PlaceOnCounter);
        }
    }

    // 6. Retrieve Bag
    void HandleRetrieveBag()
    {
        if (activeBag != null)
        {
            arrowPointer.SetTarget(activeBag.transform);
            questUI.UpdateDisplay(" Retrieve", "Collect your purchased Bag from the counter.");

            if (HasItemInInventory(bagItemID))
            {
                ChangeState(StoreState.UseBag);
            }
        }
        // Safety: If null but in inventory
        else if (HasItemInInventory(bagItemID))
        {
            ChangeState(StoreState.UseBag);
        }
    }

    // 7. Use Bag
    void HandleUseBag()
    {
        arrowPointer.DisableArrow();
        questUI.UpdateDisplay("", "Select the Bag and press <b>[E]</b> to expand your inventory!");

        if (bagWasConsumed)
        {
            FinishTutorial();
        }
    }

    // ---------------------------------------------------------
    // HELPERS
    // ---------------------------------------------------------

    GameObject FindBagOnCounter()
    {
        foreach (GameObject item in counterCollector.itemsCollected)
        {
            GroundItem gi = item.GetComponent<GroundItem>();
            if (gi != null && gi.parentPedestal.itemData.id == bagItemID)
            {
                return item;
            }
        }
        return null;
    }

    bool HasItemInInventory(int targetID)
    {
        if (playerInventory == null) return false;
        foreach (int id in playerInventory.itemIDs)
        {
            if (id == targetID) return true;
        }
        return false;
    }

    void ChangeState(StoreState newState)
    {
        currentState = newState;
        arrowPointer.DisableArrow();
    }

    void FinishTutorial()
    {
        questUI.Clear();
        Announcement.Instance.SetAnnouncement("Store Tutorial Complete!");
        arrowPointer.DisableArrow();
        currentState = StoreState.Finished;
    }
}