using UnityEngine;
using System.Collections.Generic;


public class ItemCollect : MonoBehaviour
{
    [Header("References")]
    public InventorySO inventory;
    public InventorySO equipmentInventory;
    public GameObject[] inventorySlots; // Assign in Inspector
    public GameObject equipmentSlot;
    public Transform itemHolder;
    public Transform equipmentHolder;
    public PlayerMovement movement;
    public GameObject nearItem;

    private void Update()
    {
        PlayerController player = PlayerController.Instance;
        if (player == null) return;

        HandleItemCollection(player);
        HandleItemDrop(player);
        HandleSlotVisibility(player);
    }

    private void Start()
    {
        movement = GetComponentInParent<PlayerMovement>();

        //GameObject worldItem = inventory.SpawnWorldItemByID(1, transform.position + transform.forward * 1f);
        //if (worldItem != null)
        //{
        //    ItemPickup pickup = worldItem.GetComponent<ItemPickup>();
        //    if (pickup != null)
        //    {
        //        PlaceItemInSlot(pickup.itemID, worldItem);
        //    }
        //}
    }

    private void HandleItemCollection(PlayerController player)
    {
        if (!player.onCollect || nearItem == null) return;

        ItemPickup itemPickup = nearItem.GetComponent<ItemPickup>();
        if (itemPickup == null) return;

        // Check if item is trash
        Trash trash = nearItem.GetComponent<Trash>();
        if (trash != null && trash.isBig)
        {
            Announcement announcement = FindAnyObjectByType<Announcement>();
            announcement.SetAnnouncement("Item is too Big!!");
            return; // Do not collect if trash is not big
        }

        ShrinkGun shrinkGun = nearItem.GetComponent<ShrinkGun>();
        if(shrinkGun != null)
        {

        }
        

        PlaceItemInSlot(itemPickup.itemID, nearItem);
        nearItem = null;
    }

    public void PlaceItemInSlot(int itemID, GameObject worldItem = null)
    {
        ShrinkGun shrinkGun = worldItem != null ? worldItem.GetComponent<ShrinkGun>() : null;

        if (shrinkGun != null)
        {
            PlaceEquipmentItem(itemID, worldItem);
            return;
        }


        ItemData data = inventory.itemDatabase.GetItemByID(itemID);
        if (data == null)
        {
            Debug.LogWarning("Invalid item data for ID " + itemID);
            return;
        }

        int emptySlot = inventory.GetFirstEmptySlot();
        if (emptySlot == -1)
        {
            Announcement announcement = FindAnyObjectByType<Announcement>();
            announcement.SetAnnouncement("Invenotry Full!!");
            return;
        }

        GameObject slot = GetSlot(emptySlot);
        if (slot == null)
        {
            Debug.LogError("Inventory slot not assigned!");
            return;
        }

        bool added = inventory.AddItem(itemID);
        if (!added)
        {
            Debug.Log("Could not add item to ScriptableObject inventory!");
            return;
        }

        GameObject itemObject = worldItem;

        if (itemObject == null)
        {
            if (data.prefab == null)
            {
                Debug.LogWarning($"{data.itemName} has no prefab!");
                return;
            }
            itemObject = Instantiate(data.prefab);
        }

        itemObject.transform.SetParent(slot.transform);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;

        Transform targetHolder = (data.itemType == ItemType.Equipment) ? equipmentHolder : itemHolder;
        itemObject.transform.position = targetHolder.position;
        itemObject.transform.rotation = targetHolder.rotation;

        Rigidbody rb = itemObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = itemObject.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log($"Added {data.itemName} to slot {emptySlot}");

        UpdateSlotVisibility(emptySlot);
        PlayerController.Instance.ShowItemName();
    }

    private void PlaceEquipmentItem(int itemID, GameObject worldItem = null)
    {
        ItemData data = equipmentInventory.itemDatabase.GetItemByID(itemID);
        if (data == null)
        {
            Debug.LogWarning("Invalid equipment data for ID " + itemID);
            return;
        }

        int emptySlot = equipmentInventory.GetFirstEmptySlot();
        if (emptySlot == -1)
        {
            Announcement announcement = FindAnyObjectByType<Announcement>();
            announcement.SetAnnouncement("Equipment Inventory Full!!");
            return;
        }

        GameObject slot = GetSlot(emptySlot);
        if (slot == null)
        {
            Debug.LogError("Equipment slot not assigned!");
            return;
        }

        bool added = equipmentInventory.AddItem(itemID);
        if (!added)
        {
            Debug.Log("Could not add item to equipment inventory!");
            return;
        }

        GameObject itemObject = worldItem;
        if (itemObject == null)
        {
            if (data.prefab == null)
            {
                Debug.LogWarning($"{data.itemName} has no prefab!");
                return;
            }
            itemObject = Instantiate(data.prefab);
        }

        // Parent to equipment slot
        itemObject.transform.SetParent(slot.transform);
        itemObject.transform.localPosition = Vector3.zero;
        itemObject.transform.localRotation = Quaternion.identity;

        // Move to equipment holder in world
        itemObject.transform.position = equipmentHolder.position;
        itemObject.transform.rotation = equipmentHolder.rotation;

        Rigidbody rb = itemObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = itemObject.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log($"Added {data.itemName} to EQUIPMENT slot {emptySlot}");

        UpdateSlotVisibility(emptySlot);
        PlayerController.Instance.ShowItemName();
    }


    private void HandleItemDrop(PlayerController player)
    {
        if (!player.onDrop) return;

        int activeSlot = player.slotIndex;
        GameObject slot = GetSlot(activeSlot);
        if (slot == null) return;

        Transform itemToDrop = (slot.transform.childCount > 0) ? slot.transform.GetChild(0) : null;
        if (itemToDrop == null) return;

        itemToDrop.SetParent(null);

        Rigidbody rb = itemToDrop.GetComponent<Rigidbody>();
        if (rb == null) rb = itemToDrop.gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(FindAnyObjectByType<PlayerMovement>().model.transform.forward * (5f + movement.permanentThrowBoost), ForceMode.Impulse);

        Collider col = itemToDrop.GetComponent<Collider>();
        if (col != null) col.enabled = true;

        inventory.RemoveItem(activeSlot);

        Debug.Log($"Dropped item from slot {activeSlot}");

        UpdateSlotVisibility(activeSlot);
    }

    private void HandleSlotVisibility(PlayerController player)
    {
        UpdateSlotVisibility(player.slotIndex);
    }

    public void UpdateSlotVisibility(int activeIndex)
    {
        AutoCleanEmptySlots();

        bool isAiming = PlayerController.Instance != null && PlayerController.Instance.onAim;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == null) continue;

            bool isActive = (i == activeIndex);

            // --- NEW LOGIC: HIDE IF AIMING ---
            if (isAiming)
            {
                // If aiming, force disable the visual slot, even if it's the active one
                inventorySlots[i].SetActive(false);
            }
            else
            {
                // Normal behavior: only active slot is visible
                inventorySlots[i].SetActive(isActive);
            }

            // Logic to update PlayerController flag (this should remain true logically even if hidden visually)
            // This ensures other scripts know we HAVE an item, even if we aren't showing it right now.
            if (i == activeIndex)
            {
                bool hasItem = inventorySlots[i].transform.childCount > 0;
                PlayerController.Instance.hasItemInSlot = hasItem;
            }
        }

        //for (int i = 0; i < inventorySlots.Length; i++)
        //{
        //    if (inventorySlots[i] == null) continue;

        //    bool isActive = (i == activeIndex);
        //    inventorySlots[i].SetActive(isActive);

        //    bool hasItem = inventorySlots[i].transform.childCount > 0;
        //    if (i == activeIndex)
        //        PlayerController.Instance.hasItemInSlot = hasItem;
        //}
    }

    private void AutoCleanEmptySlots()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            GameObject slot = inventorySlots[i];
            if (slot == null) continue;

            bool hasChild = slot.transform.childCount > 0;
            bool hasData = i < inventory.itemIDs.Count && inventory.itemIDs[i] != -1;

            // If slot visually empty but data says there’s still an item → fix it
            if (!hasChild && hasData)
            {
                Debug.Log($"[ItemCollect] Slot {i} was empty visually, cleaning inventory data.");
                inventory.RemoveItem(i);
            }
        }
    }


    private GameObject GetSlot(int index)
    {
        if (index < 0 || index >= inventorySlots.Length)
        {
            Debug.LogWarning($"Invalid slot index {index}");
            return null;
        }
        return inventorySlots[index];
    }

    private void OnTriggerEnter(Collider other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup != null)
        {
            nearItem = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        ItemPickup pickup = other.GetComponent<ItemPickup>();
        if (pickup != null && other.gameObject == nearItem)
        {
            nearItem = null;
        }
    }
}
