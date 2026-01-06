using UnityEngine;
using UnityEngine.UI;


public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    private int currentItemID = -1;
    private ItemDatabaseSO itemDatabase;
    public GameObject slotHighlight;

    public void Initialize(ItemDatabaseSO database)
    {
        itemDatabase = database;
    }

    public void UpdateSlot(int itemID)
    {
        currentItemID = itemID;

        if (itemDatabase == null)
            return;

        ItemData item = itemDatabase.GetItemByID(itemID);

        if (item != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public int GetItemID() => currentItemID;
}