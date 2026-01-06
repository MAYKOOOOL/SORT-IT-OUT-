using UnityEngine;

public class Bag : Consumable
{
    public InventoryUI inventoryUI;

    private bool isUsed = false;

    protected override void Start()
    {
        base.Start();
        if (inventoryUI == null)
        {
            inventoryUI = FindAnyObjectByType<InventoryUI>();
        }
    }

    public override void UseItem()
    {
        if (isUsed) return;
        isUsed = true;

        // 1. Notify the Tutorial
        StoreTutorial tutorial = FindAnyObjectByType<StoreTutorial>();
        if (tutorial != null)
        {
            tutorial.ReportBagConsumed();
        }

        // 2. Expand Inventory
        if (inventoryUI != null)
        {
            inventoryUI.AddNewSlot();
        }

        // 3. Destroy
        Destroy(gameObject);
    }
}