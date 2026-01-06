using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))] // Ensures you have a collider for the trigger
public class ExtractionTutorial : MonoBehaviour
{
    public enum TutorialState { Sorting, Extracting, CollectingReceipt, Finished }

    public Quest storeTutorialStarter;

    [Header("Garbage Setup")]
    public List<GameObject> garbageList;
    public Transform bioBin;
    public Transform nonBioBin;
    public Transform hazardousBin;

    [Header("Layer Names")]
    public string bioLayerName = "Bio";
    public string nonBioLayerName = "NonBio";
    public string hazardousLayerName = "Hazardous";

    [Header("References")]
    public Extract extractScript;
    public QuestUI questUI;
    public ArrowPointer arrowPointer;
    public PlayerController playerController;

    // Internal State
    private bool hasStarted = false; // Flag to wait for trigger
    private TutorialState currentState = TutorialState.Sorting;
    private GameObject targetTrashItem;
    private Reciept spawnedReceipt;
    public Button coinButton;
    public Trashcan[] allTrashcans;
    void Start()
    {
        // 1. Initialize References
        if (extractScript == null) extractScript = FindAnyObjectByType<Extract>();
        if (questUI == null) questUI = FindAnyObjectByType<QuestUI>();
        if (arrowPointer == null) arrowPointer = FindAnyObjectByType<ArrowPointer>();
        if (playerController == null) playerController = PlayerController.Instance;
        if (coinButton == null) coinButton = FindAnyObjectByType<RecieptObject>().getCoinButton.GetComponent<Button>();
        // 2. Disable Extract Script immediately so player can't use it before tutorial starts
        if (extractScript != null) extractScript.enabled = false;
    }

    void Update()
    {
        // Guard Clause: Don't run logic if not started or if finished
        if (!hasStarted || currentState == TutorialState.Finished) return;

        switch (currentState)
        {
            case TutorialState.Sorting:
                HandleSortingStage();
                break;
            case TutorialState.Extracting:
                HandleExtractionStage();
                break;
            case TutorialState.CollectingReceipt:
                HandleReceiptStage();
                break;
        }
    }

    // ---------------------------------------------------------
    // TRIGGER LOGIC
    // ---------------------------------------------------------
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        // SCENARIO 1: Start the Extraction Tutorial (First time touch)
        if (!hasStarted && currentState == TutorialState.Sorting)
        {
            BeginTutorial();
        }
        // SCENARIO 2: Start the Store Tutorial (Touching again after finishing)
        else if (currentState == TutorialState.Finished)
        {
            if (storeTutorialStarter != null)
            {
                // Accept the new quest. We pass 'null' as the NPC because this is a trigger.
                QuestSystem.Instance.AcceptQuest(storeTutorialStarter, null);

                Debug.Log("Extraction Tutorial Fully Complete. Next Quest Started!");

                // NOW we can destroy this script/object, as its job is 100% done
                Destroy(this);
            }
        }
    }

    void BeginTutorial()
    {
        hasStarted = true;

        // Open Quest UI
        if (questUI != null) questUI.OpenQuest();

        // Filter nulls from list just in case
        garbageList.RemoveAll(item => item == null);

        Debug.Log("Extraction Tutorial Started via Trigger!");
    }

    // ---------------------------------------------------------
    // STAGE 1 & 2: SORTING (Collect & Place)
    // ---------------------------------------------------------
    void HandleSortingStage()
    {
        // 1. Clean list of nulls (destroyed items)
        garbageList.RemoveAll(item => item == null);

        // 2. Filter out items that are already inside a trashcan
        // We create a temporary list of "remaining" items
        List<GameObject> remainingTrash = new List<GameObject>();
        foreach (var trash in garbageList)
        {
            // If trash exists AND is NOT in any bin, it is "remaining"
            if (trash != null && !IsTrashInAnyBin(trash))
            {
                remainingTrash.Add(trash);
            }
        }

        // 3. Check if we are done
        // Done if: No remaining trash on ground AND Player hands are empty
        if (remainingTrash.Count == 0 && !playerController.hasItemInSlot)
        {
            ChangeState(TutorialState.Extracting);
            return;
        }

        // 4. Logic
        if (playerController.hasItemInSlot)
        {
            // --- PLAYER HAS ITEM ---
            if (targetTrashItem != null)
            {
                Transform targetBin = GetBinByLayer(targetTrashItem.layer);
                arrowPointer.SetTarget(targetBin);

                questUI.UpdateDisplay(
                    "",
                    $"Select the Inventory Slot using Number Keys.\nPress <b>[Q]</b> to place it in the <b>{targetBin.name}</b>."
                );
            }
            else
            {
                // Fallback: If we lost track of what we picked up, just say "Sort it"
                questUI.UpdateDisplay("Sort", "Throw the item in the correct bin.");
            }
        }
        else
        {
            // --- PLAYER HANDS EMPTY ---
            // Find nearest from the *remaining* list
            targetTrashItem = GetNearestTrash(remainingTrash);

            if (targetTrashItem != null)
            {
                arrowPointer.SetTarget(targetTrashItem.transform);
                questUI.UpdateDisplay(
                    "",
                    "Go to the indicated trash object and press <b>[LEFT CLICK]</b> to Collect it."
                );
            }
        }
    }

    bool IsTrashInAnyBin(GameObject trashObj)
    {
        if (allTrashcans == null) return false;

        foreach (Trashcan bin in allTrashcans)
        {
            if (bin.trashCollected.Contains(trashObj))
            {
                return true; // Found in a bin!
            }
        }
        return false;
    }

    // ---------------------------------------------------------
    // STAGE 3: EXTRACTION
    // ---------------------------------------------------------
    void HandleExtractionStage()
    {
        if (!extractScript.enabled) extractScript.enabled = true;

        arrowPointer.SetTarget(extractScript.transform);

        questUI.UpdateDisplay(
            "",
            "Go to the Extractor Machine and press <b>[E]</b> to start extraction."
        );

        if (playerController.onInteract)
        {
            ChangeState(TutorialState.CollectingReceipt);
        }
    }

    // ---------------------------------------------------------
    // STAGE 4: RECEIPT & REWARD
    // ---------------------------------------------------------
    void HandleReceiptStage()
    {
        if (spawnedReceipt == null)
        {
            spawnedReceipt = FindAnyObjectByType<Reciept>();
            if (spawnedReceipt == null)
            {
                questUI.UpdateDisplay("", "Wait for the machine to finish.");
                arrowPointer.DisableArrow();
                return;
            }
        }

        if (playerController.onUse)
        {
            arrowPointer.DisableArrow();
            questUI.UpdateDisplay(
                "",
                "Review your results and press the <b>[CONTINUE]</b> button."
            );

            if (coinButton == null)
            {
                RecieptObject ui = FindAnyObjectByType<RecieptObject>();
                if (ui != null)
                {
                    coinButton = ui.getCoinButton.GetComponent<Button>();
                    coinButton.onClick.AddListener(FinishTutorial);
                }
            }
        }
        else
        {
            arrowPointer.SetTarget(spawnedReceipt.transform);
            questUI.UpdateDisplay(
                "",
                "Collect the Receipt and press <b>[E]</b> to open the report."
            );
        }
    }

    // ---------------------------------------------------------
    // HELPER METHODS
    // ---------------------------------------------------------

    void ChangeState(TutorialState newState)
    {
        currentState = newState;
        arrowPointer.DisableArrow();
    }

    GameObject GetNearestTrash(List<GameObject> searchList)
    {
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;
        Vector3 playerPos = playerController.transform.position;

        foreach (GameObject trash in searchList)
        {
            if (trash != null && trash.activeInHierarchy)
            {
                float dist = Vector3.Distance(trash.transform.position, playerPos);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearest = trash;
                }
            }
        }
        return nearest;
    }

    Transform GetBinByLayer(int layerIndex)
    {
        string layerName = LayerMask.LayerToName(layerIndex);

        if (layerName == bioLayerName) return bioBin;
        if (layerName == nonBioLayerName) return nonBioBin;
        if (layerName == hazardousLayerName) return hazardousBin;

        return bioBin;
    }

    public void FinishTutorial()
    {
        if (coinButton != null) coinButton.onClick.RemoveListener(FinishTutorial);
        currentState = TutorialState.Finished;
        questUI.Clear();
        Announcement.Instance.SetAnnouncement("Extraction Tutorial Complete!");
        arrowPointer.DisableArrow();
    }
}