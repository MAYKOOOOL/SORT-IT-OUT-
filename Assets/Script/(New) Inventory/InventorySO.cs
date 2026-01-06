using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Inventory/InventorySO")]
public class InventorySO : ScriptableObject
{
    [Header("Settings")]
    public ItemDatabaseSO itemDatabase;

    public int baseMinSize = 4; // The base minimum size
    public int minSize;          // The current minimum size, can be modified at runtime
    public int maxSize = 20;

    [Header("Contents")]
    public List<int> itemIDs = new List<int>(); // Each entry = ID from database (-1 = empty)

    public event Action OnInventoryChanged;

    private void OnEnable()
    {
        minSize = baseMinSize;
        if (Application.isPlaying)
        {
            // Reset minSize to baseMinSize at start
            InitializeInventory();
        }
    }

    public void InitializeInventory()
    {
        // Ensure the list has exactly minSize slots
        itemIDs.Clear();
        for (int i = 0; i < minSize; i++)
            itemIDs.Add(-1);

        OnInventoryChanged?.Invoke();
    }

    public void ClearInventory()
    {
        // Reset all slots and ensure the list matches minSize
        if (itemIDs.Count != minSize)
        {
            itemIDs.Clear();
            for (int i = 0; i < minSize; i++)
                itemIDs.Add(-1);
        }
        else
        {
            for (int i = 0; i < itemIDs.Count; i++)
                itemIDs[i] = -1;
        }

        OnInventoryChanged?.Invoke();
        Debug.Log("Inventory cleared and resized to minSize.");
    }

    public bool AddItem(int id)
    {
        int emptyIndex = itemIDs.IndexOf(-1);
        if (emptyIndex == -1)
            return false;

        itemIDs[emptyIndex] = id;
        OnInventoryChanged?.Invoke();

        return true;
    }

    public void RemoveItem(int index)
    {
        if (index >= 0 && index < itemIDs.Count)
        {
            itemIDs[index] = -1;
            OnInventoryChanged?.Invoke();

        }
    }

    public int GetFirstEmptySlot()
    {
        return itemIDs.IndexOf(-1);
    }

    public ItemData GetItemData(int index)
    {
        if (index < 0 || index >= itemIDs.Count) return null;
        int id = itemIDs[index];
        if (id < 0) return null;
        return itemDatabase.GetItemByID(id);
    }

    public GameObject SpawnWorldItemByID(int itemID, Vector3 position)
    {
        if (itemDatabase == null)
        {
            Debug.LogError("No ItemDatabase assigned to InventorySO.");
            return null;
        }

        ItemData data = itemDatabase.GetItemByID(itemID);
        if (data == null)
        {
            Debug.LogWarning("No item found in database for ID: " + itemID);
            return null;
        }

        if (data.prefab == null)
        {
            Debug.LogWarning($"Item {data.itemName} (ID {itemID}) has no prefab assigned!");
            return null;
        }

        GameObject obj = Instantiate(data.prefab, position, Quaternion.identity);

        ItemPickup pickup = obj.GetComponent<ItemPickup>();
        if (pickup == null)
            pickup = obj.AddComponent<ItemPickup>();

        pickup.itemDatabase = itemDatabase;
        pickup.itemID = itemID;

        return obj;
    }

    public GameObject SpawnWorldItemFromSlot(int slotIndex, Vector3 position, ItemCollect collector = null)
    {
        if (slotIndex < 0 || slotIndex >= itemIDs.Count) return null;
        int id = itemIDs[slotIndex];
        if (id < 0) return null;

        GameObject spawned = SpawnWorldItemByID(id, position);

        if (collector != null)
        {
            collector.PlaceItemInSlot(id, spawned);
        }

        return spawned;
    }
}
