using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private InventorySO inventory;
    [SerializeField] private ItemDatabaseSO itemDatabase;

    [Header("Slot Setup (Assign in Inspector)")]
    [SerializeField] private List<InventorySlotUI> slotList = new List<InventorySlotUI>();
    public GameObject slotPrefab;
    public GameObject slotParent;

    private void Awake()
    {
        inventory.ClearInventory();

        foreach (var slot in slotList)
        {
            if (slot != null)
                slot.Initialize(itemDatabase);
        }
    }

    private void OnEnable()
    {
        RefreshUI();
        inventory.OnInventoryChanged += RefreshUI;
    }

    private void OnDisable()
    {
        inventory.OnInventoryChanged -= RefreshUI;
    }

    private void Start()
    {

    }

    private void Update()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        for (int i = 0; i < slotList.Count; i++)
        {
            if (i < inventory.itemIDs.Count)
                slotList[i].UpdateSlot(inventory.itemIDs[i]); // ? fixed field name
            else
                slotList[i].UpdateSlot(-1);
        }

        for (int i = 0; i < slotList.Count; i++)
        {
            bool isActive = (i == PlayerController.Instance.slotIndex);
            slotList[i].slotHighlight.SetActive(isActive);
        }
    }

    public void AddNewSlot()
    {
        // 1. Make sure we have references
        if (slotPrefab == null || slotParent == null)
        {
            Debug.LogError("Slot prefab or parent not assigned!");
            return;
        }

        // 2. Instantiate a new slot
        GameObject newSlotObj = Instantiate(slotPrefab, slotParent.transform);
        InventorySlotUI newSlot = newSlotObj.GetComponent<InventorySlotUI>();

        if (newSlot == null)
        {
            Debug.LogError("Slot prefab has no InventorySlotUI component!");
            Destroy(newSlotObj);
            return;
        }

        //3. Initialize the slot with the item database
        newSlot.Initialize(itemDatabase);

        //4. Add the slot to the list
        slotList.Add(newSlot);

        // 5. Expand the inventory data (add empty entry)
        inventory.minSize++;
        inventory.itemIDs.Add(-1);
        PlayerController.Instance.currentSlotSize = inventory.minSize;

        //6. Refresh the UI to show the new slot
        RefreshUI();

        Debug.Log($"Added new inventory slot. Total slots: {slotList.Count}");
    }
}
