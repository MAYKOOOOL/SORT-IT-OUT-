using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExploreTutorial : MonoBehaviour
{
    [Header("References")]
    public QuestUI questUI;
    public ArrowPointer arrowPointer;
    public GameSystem gameSystem;

    [Header("Phase 1: Start Day")]
    public Transform ladderLocation;     // Where the player needs to go
    public Button startDayYesButton;     // The "Yes" button in the "Start Day?" UI panel
    //public GameObject startDayPanel;     // The UI panel that pops up when interacting with ladder

    [Header("Phase 2: Survive")]
    public float surviveDuration = 30f; // Optional: Tutorial day can be shorter? Or just use GameSystem timer.

    // Internal State
    private bool tutorialStarted = false;
    private bool isSurviving = false;
    private bool isFinished = false;
    public GameObject loopTutorialPanel;

    void Start()
    {
        if (questUI == null) questUI = FindAnyObjectByType<QuestUI>();
        if (arrowPointer == null) arrowPointer = FindAnyObjectByType<ArrowPointer>();
        if (gameSystem == null) gameSystem = FindAnyObjectByType<GameSystem>();

        // Setup listener for the Yes button
        if (startDayYesButton != null)
        {
            startDayYesButton.onClick.AddListener(OnDayStartedConfirmed);
        }
    }

    // Called by StoreTutorial when it finishes
    public void BeginTutorial()
    {
        tutorialStarted = true;

        // 1. Point to Ladder
        if (arrowPointer != null && ladderLocation != null)
        {
            arrowPointer.SetTarget(ladderLocation);
        }

        // 2. Update Quest UI
        if (questUI != null)
        {
            questUI.OpenQuest();
            questUI.UpdateDisplay(
                "",
                " -Go to the ladder and Press <b>[E]</b> to start your shift."
            );
        }
    }

    // This is called when the player clicks "Yes" on the Start Day UI
    void OnDayStartedConfirmed()
    {
        if (!tutorialStarted) return;

        // Clean up button listener slightly (optional if button persists)
        // startDayYesButton.onClick.RemoveListener(OnDayStartedConfirmed); 

        StartSurvivalPhase();
    }

    void StartSurvivalPhase()
    {
        isSurviving = true;

        // Disable Arrow (No specific target, just survive)
        if (arrowPointer != null) arrowPointer.DisableArrow();

        // Update Quest UI
        if (questUI != null)
        {
            questUI.UpdateDisplay(
                "",
                " -Don't let the water gauge reach 100!\n- Shrink trash to reduce the gauge.\n- Survive until the timer runs out."
            );
        }
    }

    void Update()
    {
        if (!tutorialStarted || isFinished) return;

        // Check Survival Progress
        if (isSurviving)
        {
            // Check if the day has ended in the GameSystem
            // GameSystem sets dayTimer to 0 and calls EndDay()
            if (gameSystem.dayTimer <= 0 && !gameSystem.dayStarted)
            {
                FinishTutorial();
            }
        }
    }

    void FinishTutorial()
    {
        isFinished = true;
        isSurviving = false;

        // Final Quest UI Update
        if (questUI != null)
        {
            questUI.Clear();
        }
        if(loopTutorialPanel != null)
        {
            loopTutorialPanel.SetActive(true);
        }

        Announcement.Instance.SetAnnouncement("Explore Tutorial Complete!");
        Debug.Log("Explore/Survival Tutorial Completed.");

        // Optional: Destroy this script or disable it
        // Destroy(this);
    }
}