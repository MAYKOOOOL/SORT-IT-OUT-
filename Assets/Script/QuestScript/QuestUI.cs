using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    public TextMeshProUGUI instructionsText;
    public Animator animator;
    bool isOpen = false;
    private void Start()
    {
        OpenQuest();
    }

    // If you only have one text box, we combine steps and progress into it.
    public void UpdateInstructions(string text)
    {
        instructionsText.text = text;
    }
    public void UpdateDisplay(string steps, string progress)
    {
        string final = "";

        // Format: "Steps: \n [Instructions]"
        if (!string.IsNullOrEmpty(steps))
        {
            final += "<b>Instructions:</b>\n" + steps + "\n";
        }

        // Format: "Progress: \n [Status]"
        final += "<b>Task:</b>\n" + progress;

        instructionsText.text = final;
    }

    public void OpenQuest()
    {
        if (!isOpen)
        {
            animator.SetTrigger("OpenQuest");
            isOpen = true;
        }
    }

    public void CloseQuest()
    {
        if (isOpen)
        {
            animator.SetTrigger("CloseQuest");
            isOpen = false;
        }
    }

    public void InteractQuest()
    {
        if (!isOpen)
        {
            OpenQuest();
        }
        else
        {
            CloseQuest();
        }
    }

    public void Clear()
    {
        instructionsText.text = "";
    }
}