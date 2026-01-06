using UnityEngine;

public class NPCTurnIn : MonoBehaviour
{
    // The NPCQuest component this trigger is associated with.
    public NPCQuest npcQuest;
    public bool destroy = true;
    private void OnTriggerEnter(Collider other)
    {
        // 1. Try to get the ItemPickup component from the colliding object.
        ItemPickup itemPickup = other.GetComponent<ItemPickup>();

        // Check if a valid item was found and if we have a linked NPC
        if (itemPickup != null && npcQuest != null)
        {
            QuestSystem qs = QuestSystem.Instance;

            // Exit early if there's no active quest or if this NPC is not the quest giver.
            if (qs.activeQuest == null || qs.currentQuestGiver != npcQuest) return;

            Quest activeQuest = qs.activeQuest;
            bool isTurnInQuest = false;

            // Check if the quest is any type of 'Turn In' quest
            if (activeQuest.questType == QuestType.TurnInItemID ||
                activeQuest.questType == QuestType.TurnInItemType)
            {
                isTurnInQuest = true;
            }

            if (isTurnInQuest)
            {
                bool matchesRequirement = false;

                // 2. Check Requirement based on Quest Type
                if (activeQuest.questType == QuestType.TurnInItemID)
                {
                    // Check if the item's ID matches the required item ID.
                    if (itemPickup.itemID == activeQuest.requiredItemID)
                    {
                        matchesRequirement = true;
                    }
                }
                else if (activeQuest.questType == QuestType.TurnInItemType)
                {
                    // Check if the item's Type matches the required item Type.
                    // This requires looking up the ItemData based on the itemID.
                    ItemData itemData = itemPickup.itemDatabase.GetItemByID(itemPickup.itemID);

                    if (itemData != null && itemData.itemType == activeQuest.requiredItemType)
                    {
                        matchesRequirement = true;
                    }
                }

                // 3. If the item matches, process the turn-in.
                if (matchesRequirement)
                {
                    // Update the quest progress (sends itemID for ReportItemTurnedIn to use)
                    qs.ReportItemTurnedIn(itemPickup.itemID);
                    
                    // Destroy the physical item object.
                    if (destroy)
                    {
                        Destroy(other.gameObject);
                    }
                    
                }
            }
        }
    }
}