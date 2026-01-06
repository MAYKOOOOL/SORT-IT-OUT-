using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Reciept : Consumable
{
    public List<TrashResult> results = new List<TrashResult>();

    public ItemCollect itemCollect;

    public GameObject imagePrefab;
    public GameObject recieptUI;

    public Transform bioParent;
    public Transform nonBioParent;
    public Transform hazardParent;

    public TextMeshProUGUI bioGold;
    public TextMeshProUGUI nonBioGold;
    public TextMeshProUGUI hazardGold;
    public TextMeshProUGUI totalGold;

    public GameObject getCoinButton;

    private int finalTotal = 0;
    private bool playedSequence = false;

    protected override void Start()
    {
        base.Start(); // runs the Start() in Consumable
        itemCollect = PlayerController.Instance.GetComponentInChildren<ItemCollect>();
        RecieptObject ui = FindAnyObjectByType<RecieptObject>();

        if (ui == null)
        {
            Debug.LogError("RecieptObject UI not found in scene!");
            return;
        }

        // Assign UI object
        recieptUI = ui.transform.GetChild(0).gameObject;


        // Assign parents
        bioParent = ui.bioParent;
        nonBioParent = ui.nonBioParent;
        hazardParent = ui.hazardParent;

        // Assign text fields
        bioGold = ui.bioGold;
        nonBioGold = ui.nonBioGold;
        hazardGold = ui.hazardGold;
        totalGold = ui.totalGold;

        // Assign button
        getCoinButton = ui.getCoinButton;
        getCoinButton.GetComponent<Button>().onClick.AddListener(GetCoin);
    }

    public override void UseItem()
    {
        recieptUI.SetActive(true);
        
        if (!playedSequence)
        {
            playedSequence = true;
            StartCoroutine(PlaySequence());
        }
    }

    IEnumerator PlaySequence()
    {
        getCoinButton.SetActive(false);

        int bio = 0;
        int nonBio = 0;
        int hazard = 0;

        foreach (Transform t in bioParent) Destroy(t.gameObject);
        foreach (Transform t in nonBioParent) Destroy(t.gameObject);
        foreach (Transform t in hazardParent) Destroy(t.gameObject);

        bioGold.text = "0G";
        nonBioGold.text = "0G";
        hazardGold.text = "0G";
        totalGold.text = "0G";

        // Play each item one by one
        foreach (var r in results)
        {
            Transform parent = r.type == TrashType.Bio ? bioParent :
                              r.type == TrashType.NonBio ? nonBioParent :
                              hazardParent;

            GameObject img = Instantiate(imagePrefab, parent);

            Image trashImg = img.transform.GetChild(0).GetComponent<Image>();
            Image bg = img.GetComponent<Image>();

            trashImg.sprite = r.sprite;

            // Wait a bit before coloring
            yield return new WaitForSeconds(0.25f);

            if (r.correct)
                bg.color = Color.lightGreen;
            else
                bg.color = Color.darkRed;

            // add gold
            if (r.type == TrashType.Bio) bio += r.value;
            if (r.type == TrashType.NonBio) nonBio += r.value;
            if (r.type == TrashType.Hazardous) hazard += r.value;

            // update UI
            bioGold.text = bio.ToString();
            nonBioGold.text = nonBio.ToString();
            hazardGold.text = hazard.ToString();

            finalTotal = bio + nonBio + hazard;
            totalGold.text = finalTotal.ToString();

            yield return new WaitForSeconds(0.5f);
        }

        yield return new WaitForSeconds(0.5f);
        getCoinButton.SetActive(true);
    }

    // Button calls this
    public void GetCoin()
    {
        if (QuestSystem.Instance != null && QuestSystem.Instance.activeQuest != null)
        {
            int totalExtracted = results.Count;
            int correctExtracted = 0;

            foreach (var r in results)
            {
                if (r.correct)
                {
                    correctExtracted++;
                }
            }

            // Report both counts to the QuestSystem
            QuestSystem.Instance.ReportExtractedItems(totalExtracted, correctExtracted);
        }
        if (finalTotal > 0)
        {
            playerStats.UpdateCoins(finalTotal);
        }
        else
        {
            // Negative: subtract directly
            if (playerStats != null)
                playerStats.UpdateCoins(finalTotal);
        }

        ExtractionTutorial extractionTutorial = FindAnyObjectByType<ExtractionTutorial>();
        if(extractionTutorial != null)
        {
            extractionTutorial.FinishTutorial();
        }

        // Hide the receipt UI and remove the receipt item
        recieptUI.SetActive(false);
        Destroy(gameObject);
    }
}