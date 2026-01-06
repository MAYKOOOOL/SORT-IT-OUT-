using System.Collections.Generic;
using UnityEngine;

public class Trashbag : MonoBehaviour
{
    //public GameObject innerBagPanel;
    //public InventoryObject inventoryTemplate;

    //[SerializeField] private InventoryObject myInventory;
    //private DynamicInterface bagUI;
    //private bool isOpen = false;

    //void Start()
    //{
    //    innerBagPanel = GameObject.Find("Bag Panel");

    //    if (innerBagPanel == null)
    //    {
    //        Debug.LogError("Bag Panel not found!");
    //        return;
    //    }

    //    bagUI = innerBagPanel.GetComponentInChildren<DynamicInterface>(true);
    //    if (bagUI == null)
    //    {
    //        Debug.LogError("DynamicInterface not found on Bag Panel.");
    //        return;
    //    }

    //    // Create a fresh instance of the inventory
    //    myInventory = ScriptableObject.Instantiate(inventoryTemplate);
    //    myInventory.Container = new Inventory();
    //    for (int i = 0; i < myInventory.Container.Items.Length; i++)
    //    {
    //        myInventory.Container.Items[i] = new InventorySlot();
    //    }
    //}

    //public void Use()
    //{
    //    isOpen = !isOpen;

    //    if (isOpen)
    //    {
    //        // Swap the inventory data to this bag's data
    //        bagUI.inventory = myInventory;

    //        // Make sure each slot points back to this UI
    //        for (int i = 0; i < myInventory.Container.Items.Length; i++)
    //        {
    //            myInventory.Container.Items[i].parent = bagUI;
    //        }

    //        // Update the visuals
    //        bagUI.UpdateSlots();
    //    }

    //    // Toggle only children (not the whole panel)
    //    foreach (Transform child in innerBagPanel.transform)
    //    {
    //        child.gameObject.SetActive(isOpen);
    //    }
    //}

    //public void ReleaseItems()
    //{
    //    List<Transform> children = new List<Transform>();

    //    foreach (Transform child in transform)
    //    {
    //        if (child.GetComponent<GroundItem>() != null)
    //        {
    //            children.Add(child);
    //        }
    //    }

    //    foreach (Transform item in children)
    //    {
    //        item.SetParent(null);

    //        Rigidbody rb = item.GetComponent<Rigidbody>();
    //        if (rb != null)
    //        {
    //            rb.isKinematic = false;
    //            rb.detectCollisions = true;

    //            Vector3 forceDirection = (Vector3.up + Random.insideUnitSphere * 0.5f).normalized;
    //            float forceAmount = Random.Range(3f, 6f);
    //            rb.AddForce(forceDirection * forceAmount, ForceMode.Impulse);
    //        }

    //        item.gameObject.SetActive(true);
    //    }

    //    myInventory.Clear();
    //    bagUI?.UpdateSlots();
    //}

    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
    //        return;

    //    GroundItem groundItem = collision.gameObject.GetComponent<GroundItem>();
    //    Trash trash = collision.gameObject.GetComponent<Trash>();

    //    if (groundItem == null || (groundItem.item.type == ItemType.Trash && trash.isBig))
    //        return;

    //    // Add item to the inventory
    //    Item itemToAdd = new Item(groundItem.item);
    //    InventorySlot slot = myInventory.AddItem(itemToAdd, 1, collision.gameObject);

    //    if (slot != null)
    //    {
    //        GameObject collectedItem = groundItem.gameObject;
    //        collectedItem.transform.SetParent(this.transform);
    //        collectedItem.transform.localPosition = Vector3.zero;
    //        collectedItem.transform.localRotation = Quaternion.identity;

    //        Rigidbody rb = collectedItem.GetComponent<Rigidbody>();
    //        if (rb != null)
    //        {
    //            rb.isKinematic = true;
    //            rb.detectCollisions = false;
    //        }

    //        collectedItem.SetActive(false);

    //        if (isOpen)
    //            bagUI.UpdateSlots();
    //    }
    //    else
    //    {
    //        Debug.Log("Trashbag inventory full");
    //    }

    //}
}
