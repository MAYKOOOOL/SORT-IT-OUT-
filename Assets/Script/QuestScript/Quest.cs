using UnityEngine;

public enum QuestType
{
    TurnInItemID,     // NEW: Collect/Throw items with a specific ID.
    TurnInItemType,   // NEW: Collect/Throw items of a specific type (e.g., any Equipment).
    CheckInventory,   // Have items in inventory (do not remove them)
    GoToLocation,     // Walk into a specific trigger
    DestroyEnemy,
    TotalItemsExtracted,   // NEW: Track the total count of trash processed.
    CorrectItemsExtracted
}

[CreateAssetMenu(menuName = "Quest/New Quest")]
public class Quest : ScriptableObject
{
    [Header("Info")]
    public string questTitle;
    [TextArea] public string instructions;
    [TextArea] public string startDialogue;
    [TextArea] public string completionDialogue;

    [Header("Settings")]
    public QuestType questType;
    public bool showArrow = true;
    public bool autoComplete = false;

    [Header("Requirements")]
    public ItemType requiredItemType;       // NEW: Used by TurnInItemType quests.
    public int requiredItemID = -1;        // Used by TurnInItemID quests.
    public int requiredAmount = 1;

    [Header("Rewards & Start Items")]
    public int initialItemID = -1;
    public int rewardItemID = -1;
}