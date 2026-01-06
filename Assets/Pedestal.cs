using UnityEngine;
using TMPro;

public class Pedestal : MonoBehaviour
{
    [Header("State")]
    public bool purchased;
    public ItemData itemData;

    [Header("Display")]
    public Transform spawnPoint;
    public TextMeshProUGUI costText;
    private GameObject currentItem;

    public void SpawnItem(ItemData data)
    {
        if (data == null) return;

        purchased = false;
        itemData = data;

        if (currentItem != null)
            Destroy(currentItem);

        currentItem = Instantiate(data.prefab, spawnPoint.position, spawnPoint.rotation);

        var groundItem = currentItem.GetComponent<GroundItem>();
        groundItem.parentPedestal = this;
        groundItem.purchased = false;
        costText.text = $"{data.price}G";
    }

    public void MarkPurchased()
    {
        purchased = true;
        costText.text = "SOLD";
        currentItem = null;
    }

    public void ClearPedestal()
    {
        purchased = false;
        itemData = null;
        if (currentItem != null) Destroy(currentItem);

        costText.text = "";
    }
}
