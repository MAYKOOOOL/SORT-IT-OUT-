using UnityEngine;
using TMPro;

public class NPCQuest : MonoBehaviour
{
    [Header("Settings")]
    public bool autoAcceptOnTrigger = false;

    [Header("Quest List")]
    public Quest[] quests;
    public int currentQuestIndex = 0; // Tracks which quest we are on

    [Header("References")]
    public GameObject[] questIcons; // 0: Has Quest (!), 1: Waiting (?), 2: Reward Ready (Gold ?)
    public Dialogue dialogueSystem;
    public TextMeshProUGUI interactText;

    private bool playerInRange;
    private bool hasGivenCurrentQuest; // Prevents re-giving the same quest
    private bool completionSequenceRunning = false;
    private void Start()
    {
        UpdateIcons();
    }

    private void Update()
    {
        if (playerInRange && PlayerController.Instance.onInteract) // Or PlayerController.Instance.onInteract
        {
            Interact();
        }
    }

    void Interact()
    {
        if(completionSequenceRunning) return;
        // 1. If no quests left
        if (currentQuestIndex >= quests.Length)
        {
            dialogueSystem.ShowGenericDialogue("I have nothing else for you. Safe travels!");
            return;
        }

        Quest q = quests[currentQuestIndex];
        bool isMyQuestActive = (QuestSystem.Instance.activeQuest == q);

        // --- STATE 1: OFFER QUEST ---
        if (!hasGivenCurrentQuest && !isMyQuestActive)
        {
            // Open Dialogue with Yes/No callbacks
            dialogueSystem.OpenDialogue(q.startDialogue, OnAcceptQuest, null);
        }
        // --- STATE 2: WAITING FOR COMPLETION ---
        else if (isMyQuestActive && !QuestSystem.Instance.isReadyToReturn)
        {
            dialogueSystem.ShowGenericDialogue("Check your quest log for instructions.");
        }
        // --- STATE 3: COMPLETE & REWARD ---
        else if (isMyQuestActive && QuestSystem.Instance.isReadyToReturn)
        {
            completionSequenceRunning = true;
            // Show completion text, then trigger reward logic on close
            dialogueSystem.ShowGenericDialogue(q.completionDialogue);
            dialogueSystem.OnDialogueClose += FinishQuestSequence;
        }
    }

    // Callback for "Yes" button
    void OnAcceptQuest()
    {
        StartQuestLogic();
    }

    // Shared logic for both Manual and Auto acceptance
    void StartQuestLogic()
    {
        QuestSystem.Instance.AcceptQuest(quests[currentQuestIndex], this);
        hasGivenCurrentQuest = true;
        UpdateIcons();
    }

    // Called after completion dialogue closes
    void FinishQuestSequence()
    {
        dialogueSystem.OnDialogueClose -= FinishQuestSequence; // Unsubscribe

        GiveReward();

        QuestSystem.Instance.CompleteActiveQuest(); // Tell system to clear
        currentQuestIndex++;
        Debug.Log("TANGIMMOHAYOPKA");// Move to next quest
        hasGivenCurrentQuest = false; // Reset for next one
        completionSequenceRunning = false;
        UpdateIcons();
    }

    public void GiveReward()
    {
        if (currentQuestIndex < quests.Length)
        {
            int rID = quests[currentQuestIndex].rewardItemID;
            if (rID != -1)
            {
                // Throw reward towards player
                Vector3 spawnPos = transform.position + transform.forward + Vector3.up;
                GameObject itemObj = QuestSystem.Instance.playerInventory.SpawnWorldItemByID(rID, spawnPos);

                // Add force if it has a rigidbody
                if (itemObj != null && itemObj.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce(transform.forward * 3f + Vector3.up * 2f, ForceMode.Impulse);
                }
            }
        }
    }

    public void UpdateIcons()
    {
        // Hide all first
        if (questIcons.Length > 0) foreach (var icon in questIcons) icon.SetActive(false);

        if (currentQuestIndex >= quests.Length) return;

        Quest q = quests[currentQuestIndex];
        bool isMyQuestActive = (QuestSystem.Instance.activeQuest == q);

        if (!hasGivenCurrentQuest)
        {
            if (questIcons.Length > 0) questIcons[0].SetActive(true); // Exclamation (!)
        }
        else if (isMyQuestActive && !QuestSystem.Instance.isReadyToReturn)
        {
            if (questIcons.Length > 1) questIcons[1].SetActive(true); // Grey Question (?)
        }
        else if (isMyQuestActive && QuestSystem.Instance.isReadyToReturn)
        {
            if (questIcons.Length > 2) questIcons[2].SetActive(true); // Gold Question (?)
        }
    }
    public void AdvanceQuestIndexExternal()
    {
        // 1. Move to next quest
        currentQuestIndex++;

        // 2. Reset flags so we can give the new quest
        hasGivenCurrentQuest = false;
        completionSequenceRunning = false;

        // 3. Update Visuals
        UpdateIcons();

        Debug.Log($"{name} advanced to quest index {currentQuestIndex} via Auto-Complete.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // 1. Check if we should Auto Accept
            if (autoAcceptOnTrigger)
            {
                TryAutoAccept();
            }
            // 2. Otherwise show the "Press E" text
            else
            {
                if (interactText) interactText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactText) interactText.gameObject.SetActive(false);
        }
    }

    private void TryAutoAccept()
    {
        // Validation checks
        if (currentQuestIndex >= quests.Length) return;
        Quest q = quests[currentQuestIndex];

        bool isMyQuestActive = (QuestSystem.Instance.activeQuest == q);

        // Only accept if we haven't given it yet AND it's not already active
        if (!hasGivenCurrentQuest && !isMyQuestActive)
        {
            Debug.Log("Auto-Accepting Quest via Trigger...");
            StartQuestLogic(); // Bypasses the Dialogue System
        }
    }
}