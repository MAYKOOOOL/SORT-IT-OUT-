using UnityEngine;
using System.Collections;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance;

    [Header("References")]
    public QuestUI questUI;
    public InventorySO playerInventory;
    public ArrowPointer arrowPointer;

    [Header("Current Status")]
    public Quest activeQuest;
    public NPCQuest currentQuestGiver; // The NPC we need to return to
    public int currentProgressAmount;
    public bool isReadyToReturn; // True when objectives are met, waiting to talk to NPC
    public Transform tutorialTarget; // Set only at start for tutorial purposes
    [TextArea]public string startInstructions;

    [Header("Player Transform")]
    public Transform playerTransform;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Subscribe to inventory changes to auto-update progress
        if (playerInventory != null)
            playerInventory.OnInventoryChanged += CheckQuestProgress;

        if (tutorialTarget != null)
        {
            arrowPointer.SetTarget(tutorialTarget);
            questUI.UpdateInstructions(startInstructions);

            // Safety: Clear the variable so we never use it again by accident
            tutorialTarget = null;
        }
    }

    private void Update()
    {
        // We only update the arrow target if a quest is active and the arrow is requested
        if (activeQuest != null && activeQuest.showArrow && !isReadyToReturn)
        {
            UpdateArrowTarget();
        }
    }

    private void OnDestroy()
    {
        if (playerInventory != null)
            playerInventory.OnInventoryChanged -= CheckQuestProgress;
    }

    // --- PHASE 1: ACCEPT QUEST ---
    public void AcceptQuest(Quest newQuest, NPCQuest npc)
    {
        questUI.OpenQuest();
        activeQuest = newQuest;
        currentQuestGiver = npc;
        currentProgressAmount = 0;
        isReadyToReturn = false;

        // Give Initial Item (if any)
        if (activeQuest.initialItemID != -1)
        {
            // Calculate the direction and position to throw from the NPC towards the player
            Vector3 spawnPos = npc.transform.position + npc.transform.forward + Vector3.up;
            Vector3 throwDir = (playerTransform.position - spawnPos).normalized;
            throwDir.y = 0; // Keep the throw flat

            // Use the new helper to spawn and throw the item
            ThrowItemToPlayer(activeQuest.initialItemID, spawnPos, throwDir);

            // IMPORTANT: The player must pick up the item. 
            // We DO NOT call playerInventory.AddItem here.
        }

        // Initialize Arrow
        if (!activeQuest.showArrow) arrowPointer.DisableArrow();

        // Check immediately (in case we already have the items)
        CheckQuestProgress();

        if (activeQuest.showArrow)
        {
            UpdateArrowTarget();
        }
        UpdateUI();
    }

    // --- PHASE 2: TRACK PROGRESS ---
    // --- PHASE 2: TRACK PROGRESS ---
    public void CheckQuestProgress()
    {
        if (activeQuest == null || isReadyToReturn) return;

        bool requirementsMet = false;

        switch (activeQuest.questType)
        {
            case QuestType.TotalItemsExtracted:
            case QuestType.CorrectItemsExtracted:
                if (currentProgressAmount >= activeQuest.requiredAmount) requirementsMet = true;
                break;

            case QuestType.CheckInventory:
                // ALWAYS use CheckInventory for type counting as requested.
                currentProgressAmount = CountItemsInInventoryByType(activeQuest.requiredItemType);
                if (currentProgressAmount >= activeQuest.requiredAmount) requirementsMet = true;
                break;

            case QuestType.TurnInItemID:
            // Check progress for specific item ID turn-in (relies on physical item throw count)
            case QuestType.TurnInItemType:
                // Check progress for item type turn-in (relies on physical item throw count)
                if (currentProgressAmount >= activeQuest.requiredAmount) requirementsMet = true;
                break;

            case QuestType.GoToLocation:
                if (currentProgressAmount >= 1) requirementsMet = true;
                break;
            case QuestType.DestroyEnemy:
                if (currentProgressAmount >= activeQuest.requiredAmount) requirementsMet = true;
                break;
        }

        UpdateUI();

        if (requirementsMet)
        {
            // --- NEW LOGIC START ---
            if (activeQuest.autoComplete)
            {
                HandleAutoComplete();
            }
            else
            {
                SetReadyToReturn();
            }
            // --- NEW LOGIC END ---
            currentQuestGiver.UpdateIcons();
        }
        
    }

    private void HandleAutoComplete()
    {
        Debug.Log($"Auto-Completing Quest: {activeQuest.questTitle}");

        // 1. Give Reward immediately (spawn at player's feet or throw up)
        if (activeQuest.rewardItemID != -1)
        {
            ThrowItemToPlayer(activeQuest.rewardItemID, playerTransform.position + Vector3.up * 2, Vector3.up);
        }

        // 2. Notify NPC so they advance their index (IMPORTANT)
        if (currentQuestGiver != null)
        {
            currentQuestGiver.AdvanceQuestIndexExternal();
        }

        // 3. Clear System
        CompleteActiveQuest();
    }

    // Helper to count specific items in your ID list
    private int CountItemsInInventoryByType(ItemType targetType)
    {
        int count = 0;

        // We must iterate through the player's item IDs and look up the ItemType for each one.
        foreach (int id in playerInventory.itemIDs)
        {
            ItemData d = playerInventory.itemDatabase.GetItemByID(id);
            if (d != null && d.itemType == targetType)
            {
                count++;
            }
        }
        return count;
    }

    // Called by GoToTrigger
    public void ReportLocationReached(Quest locationQuest)
    {
        if (activeQuest == locationQuest)
        {
            currentProgressAmount = 1; // 1/1 Visited
            CheckQuestProgress();
        }
    }

    // --- PHASE 3: RETURN TO NPC ---
    private void SetReadyToReturn()
    {
        isReadyToReturn = true;

        // Point arrow back to NPC
        if (activeQuest.showArrow && currentQuestGiver != null)
        {
            arrowPointer.SetTarget(currentQuestGiver.transform);
        }
        string returnMessage = $"Quest finished! Go back to {currentQuestGiver.name}";

        // We pass the quest instructions, but use the new message for the progress area
        questUI.UpdateDisplay(activeQuest.instructions, returnMessage);
    }

    // --- PHASE 4: COMPLETE & REWARD ---
    public void CompleteActiveQuest()
    {
        if (activeQuest == null) return;

        // Item removal logic removed here because:
        // 1. CheckInventory items are kept.
        // 2. TurnInItem items are destroyed by the NPCTurnIn script upon collision.

        // Reset System
        activeQuest = null;
        isReadyToReturn = false;
        currentQuestGiver = null;

        questUI.Clear();
        arrowPointer.DisableArrow();
    }

    public void ReportItemTurnedIn(int itemID)
    {
        if (activeQuest == null || isReadyToReturn) return;

        bool matchesRequirement = false;

        // 1. Check if the thrown item ID matches the required ID (for TurnInItemID)
        if (activeQuest.questType == QuestType.TurnInItemID)
        {
            if (activeQuest.requiredItemID == itemID)
            {
                matchesRequirement = true;
            }
        }
        // 2. Check if the thrown item type matches the required type (for TurnInItemType)
        else if (activeQuest.questType == QuestType.TurnInItemType)
        {
            ItemData d = playerInventory.itemDatabase.GetItemByID(itemID);
            if (d != null && d.itemType == activeQuest.requiredItemType)
            {
                matchesRequirement = true;
            }
        }

        if (matchesRequirement)
        {
            currentProgressAmount++; // Increment count based on the thrown item
            CheckQuestProgress();
        }
    }

    public void ReportEnemyDestroyed(int placeholderID) // placeholderID is still ignored
    {
        // The main check (activeQuest == null) is still good for safety
        if (activeQuest == null || isReadyToReturn) return;

        // This method is now implicitly guaranteed to be called only for DestroyEnemy quests,
        // so we can directly increment the progress.
        currentProgressAmount++;

        // Check if this kill meets the final requirements
        CheckQuestProgress();
    }

    public void ReportExtractedItems(int totalCount, int correctCount)
    {
        if (activeQuest == null || isReadyToReturn) return;

        // We check the quest type and add the relevant count
        if (activeQuest.questType == QuestType.TotalItemsExtracted)
        {
            currentProgressAmount += totalCount;
        }
        else if (activeQuest.questType == QuestType.CorrectItemsExtracted)
        {
            currentProgressAmount += correctCount;
        }

        // Check if the goal is met
        CheckQuestProgress();
    }

    private void UpdateArrowTarget()
    {
        // The arrow is already pointed at the NPC or a fixed tutorial target, so skip.
        if (activeQuest.questType == QuestType.GoToLocation ||
            activeQuest.questType == QuestType.TurnInItemID ||
            activeQuest.questType == QuestType.TurnInItemType)
        {
            // GoToLocation is assumed to set its own target elsewhere (the location trigger).
            // TurnInItem quests should point back to the NPC only when isReadyToReturn is true.
            return;
        }

        Transform bestTarget = null;

        if (activeQuest.questType == QuestType.DestroyEnemy)
        {
            // Find the nearest enemy with the ipis script
            bestTarget = FindNearestEnemy();
        }
        else if (activeQuest.questType == QuestType.CheckInventory)
        {
            // Find the nearest item pickup that matches the required type
            bestTarget = FindNearestItemOfType(activeQuest.requiredItemType);
        }

        if (bestTarget != null)
        {
            // Set the new dynamic target
            arrowPointer.SetTarget(bestTarget);
        }
        else
        {
            // If no targets are found (e.g., all enemies are dead or no items exist), disable the arrow.
            arrowPointer.DisableArrow();
        }
    }

    private Transform FindNearestEnemy()
    {
        // NOTE: This uses the highly efficient FindObjectsByType (Unity 2022+), or you can use FindObjectsOfType<ipis>()
        // Assuming 'ipis' is the script name for your enemy.
        ipis[] allEnemies = FindObjectsByType<ipis>(FindObjectsSortMode.None);

        Transform nearest = null;
        float minDistanceSqr = float.MaxValue;

        foreach (ipis enemy in allEnemies)
        {
            if (enemy.transform == playerTransform) continue; // Skip the player if player has ipis script

            float distanceSqr = (enemy.transform.position - playerTransform.position).sqrMagnitude;
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                nearest = enemy.transform;
            }
        }
        return nearest;
    }

    private Transform FindNearestItemOfType(ItemType targetType)
    {
        // Find all ItemPickup objects in the scene.
        ItemPickup[] allPickups = FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

        Transform nearest = null;
        float minDistanceSqr = float.MaxValue;

        foreach (ItemPickup pickup in allPickups)
        {
            // Check if this item matches the required type
            ItemData d = playerInventory.itemDatabase.GetItemByID(pickup.itemID);

            if (d != null && d.itemType == targetType)
            {
                float distanceSqr = (pickup.transform.position - playerTransform.position).sqrMagnitude;
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    nearest = pickup.transform;
                }
            }
        }
        return nearest;
    }

    public void ThrowItemToPlayer(int itemID, Vector3 spawnPosition, Vector3 throwDirection)
    {
        if (itemID == -1) return;

        // Spawn the item using the InventorySO's helper
        GameObject itemObj = playerInventory.SpawnWorldItemByID(itemID, spawnPosition);

        // Add force if it has a rigidbody
        if (itemObj != null && itemObj.TryGetComponent(out Rigidbody rb))
        {
            // Use the provided direction for consistency
            rb.AddForce(throwDirection * 3f + Vector3.up * 2f, ForceMode.Impulse);
        }

        Debug.Log($"Threw item ID {itemID} towards the player.");
    }

    private void UpdateUI()
    {
        if (activeQuest == null) return;

        if (isReadyToReturn) return; // Handled in SetReadyToReturn

        string progressStr;
        string objectiveName;

        // --- Case 1: Go To Location ---
        if (activeQuest.questType == QuestType.GoToLocation)
        {
            progressStr = "Travel to Pointed location";
        }
        // --- Case 2: Destroy Enemy ---
        else if (activeQuest.questType == QuestType.DestroyEnemy)
        {
            // Display "Enemies" followed by the progress count.
            // As requested: "Enemies: 3/5"
            string enemyName = "Enemies shrinked";
            progressStr = $"{enemyName}: {currentProgressAmount}/{activeQuest.requiredAmount}";
        }
        else if (activeQuest.questType == QuestType.TotalItemsExtracted ||
             activeQuest.questType == QuestType.CorrectItemsExtracted)
        {
            if (activeQuest.questType == QuestType.TotalItemsExtracted)
                objectiveName = "Items Extracted";
            else // CorrectItemsExtracted
                objectiveName = "Correctly Sorted";

            progressStr = $"{objectiveName}: {currentProgressAmount}/{activeQuest.requiredAmount}";
        }
        // --- Case 3 & 4: Item Quests (TurnIn/CheckInventory) ---
        else // Handles TurnInItemID, TurnInItemType, and CheckInventory
        {
            string iName;
            int requiredID = activeQuest.requiredItemID;

            // Determine the required name based on quest type
            if (activeQuest.questType == QuestType.TurnInItemType || activeQuest.questType == QuestType.CheckInventory)
            {
                // Use the ItemType enum name (e.g., "Equipment")
                iName = activeQuest.requiredItemType.ToString();
            }
            else // Must be TurnInItemID (uses requiredItemID)
            {
                // Get item name for display using the ItemDatabase (as requested)
                ItemData d = playerInventory.itemDatabase.GetItemByID(requiredID);

                // Check if ItemData exists; otherwise, use a fallback.
                iName = (d != null) ? d.itemName : "Specific Item";
            }

            // Final formatting for all item quests
            progressStr = $"{iName} collected: {currentProgressAmount}/{activeQuest.requiredAmount}";
        }

        questUI.UpdateDisplay(activeQuest.instructions, progressStr);
    }

    public void SkipTutorial()
    {
        activeQuest = null;
        arrowPointer.currentTarget = null;
        arrowPointer.arrowGraphic.gameObject.SetActive(false);
        Announcement announcement = FindAnyObjectByType<Announcement>();
        announcement.Resume();
        questUI.Clear();
        questUI.CloseQuest();
    }
}