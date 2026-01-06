using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public Image npcIconHolder;
    public TextMeshProUGUI dialogueText;
    public GameObject panel;
    public GameObject buttons;
    public Button yesButton;
    public Button noButton;
    public Button nextButton;



    public void OpenDialogue()
    {
        panel.SetActive(true);
    }

    public void RewardDailogue()
    {
        panel.SetActive(true);
    }

    void ShowAcceptButtons()
    {
        nextButton.gameObject.SetActive(false);
        buttons.SetActive(true);

        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(OnAccept);

        noButton.onClick.RemoveAllListeners();
        noButton.onClick.AddListener(Close);
    }

    void OnAccept()
    {
        Close();
    }

    // Add to Dialogue.cs

    // Variable to store what happens when the dialogue closes
    public System.Action OnDialogueClose;

    // 1. For simple messages (No Yes/No buttons)
    public void ShowGenericDialogue(string text)
    {
        panel.SetActive(true);
        dialogueText.text = text;

        buttons.SetActive(false); // Hide Yes/No
        nextButton.gameObject.SetActive(true); // Show Close/Next

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(Close);
    }

    // 2. For Quest Offers (Yes/No buttons)
    public void OpenDialogue(string text, UnityEngine.Events.UnityAction onAccept, UnityEngine.Events.UnityAction onDecline)
    {
        panel.SetActive(true);
        dialogueText.text = text;

        buttons.SetActive(true); // Show Yes/No
        nextButton.gameObject.SetActive(false); // Hide Next

        yesButton.onClick.RemoveAllListeners();
        if (onAccept != null) yesButton.onClick.AddListener(onAccept);
        yesButton.onClick.AddListener(Close);

        noButton.onClick.RemoveAllListeners();
        if (onDecline != null) noButton.onClick.AddListener(onDecline);
        noButton.onClick.AddListener(Close);
    }

    public void Close()
    {
        buttons.SetActive(false);
        panel.SetActive(false);

        // Trigger the close event if anything is listening (like the Reward trigger)
        OnDialogueClose?.Invoke();
    }
}
