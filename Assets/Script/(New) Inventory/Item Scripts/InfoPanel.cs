using TMPro;
using UnityEngine;

public class InfoPanel : MonoBehaviour
{
    public GameObject infoPanel;
    public TextMeshProUGUI interactText;
    bool nearPlayer;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(nearPlayer && PlayerController.Instance.onInteract)
        {
            infoPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearPlayer = true;
            interactText.gameObject.SetActive(true);
            interactText.text = "Press E to view Info Panel";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            nearPlayer = false;
            interactText.gameObject.SetActive(false);
        }
    }
}
