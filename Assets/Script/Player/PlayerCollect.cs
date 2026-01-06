using UnityEngine;

//[RequireComponent(typeof(CharacterController))]
//public class PlayerCollect : MonoBehaviour
//{
//    public InventoryObject inventory;
//    public LayerMask itemLayer;
//    public GameObject nearItem;

//    private void Update()
//    {
//        if (PlayerController.Instance.onMove)
//            FindNearestItem();

//        if (PlayerController.Instance.onCollect && nearItem)
//        {
//            TryCollectItem();
//        }
//    }

//    public void TryCollectItem()
//    {
//        var groundItem = nearItem.GetComponent<GroundItem>();
//        if (!groundItem) return;

//        // Optional: Skip big trash
//        if (groundItem.item is TrashObject)
//        {
//            Trash trash = nearItem.GetComponent<Trash>();
//            if (trash != null && trash.isBig)
//                return;
//        }

//        // Create Item data from ItemObject
//        Item item = new Item(groundItem.item);
//        GameObject collectedObj = nearItem;

//        // Disable item in world
//        collectedObj.transform.SetParent(null);
//        collectedObj.SetActive(false);

//        // Cleanup physics
//        Rigidbody rb = collectedObj.GetComponent<Rigidbody>();
//        if (rb != null)
//        {
//            rb.isKinematic = true;
//            rb.detectCollisions = false;
//        }

//        collectedObj.layer = 0;

//        // Add to inventory and link object
//        InventorySlot slot = inventory.AddItem(item, 1, collectedObj);

//        if (slot != null)
//        {
//            int index = System.Array.IndexOf(inventory.Container.Items, slot);
//            //PlayerController.Instance.AssignItemToSlot(index, collectedObj);
//            nearItem = null;
//        }
//        else
//        {
//            Debug.Log("Inventory full!");
//            collectedObj.SetActive(true);
//        }
//    }

//    private void FindNearestItem()
//    {
//        Collider[] hits = Physics.OverlapSphere(transform.position, 2f);
//        float closest = Mathf.Infinity;
//        nearItem = null;

//        foreach (var hit in hits)
//        {
//            if (hit.TryGetComponent(out GroundItem item))
//            {
//                float dist = Vector3.Distance(transform.position, hit.transform.position);
//                if (dist < closest)
//                {
//                    closest = dist;
//                    nearItem = hit.gameObject;
//                }
//            }
//        }
//    }

//    private void OnApplicationQuit()
//    {
//        inventory.Container.Items = new InventorySlot[inventory.Container.Items.Length];
//    }
//}
