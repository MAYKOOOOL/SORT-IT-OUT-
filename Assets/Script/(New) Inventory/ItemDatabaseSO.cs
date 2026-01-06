using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabaseSO : ScriptableObject
{
    public List<ItemData> items = new List<ItemData>();

    // Auto-assign IDs whenever you change the list in the Inspector
    private void OnValidate()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null)
                items[i].id = i;
        }
    }

    public ItemData GetItemByID(int id)
    {
        if (id < 0 || id >= items.Count)
            return null;
        return items[id];
    }

    public List<ItemData> GetItemsByType(ItemType type)
    {
        List<ItemData> result = new List<ItemData>();

        foreach (var item in items)
        {
            if (item != null && item.itemType == type)
                result.Add(item);
        }

        return result;
    }
}