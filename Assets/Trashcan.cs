using UnityEngine;
using System.Collections.Generic;

public class Trashcan : MonoBehaviour
{
    public TrashType acceptedTrash;
    PlayerStats playerStats;

    public List<GameObject> trashCollected = new List<GameObject>();
    private Collider myCollider;
    void Start()
    {
        myCollider = GetComponent<Collider>();
        playerStats = FindAnyObjectByType<PlayerStats>();
    }
    private void Update()
    {
        // Clean objects that are no longer inside or got destroyed/disabled
        for (int i = trashCollected.Count - 1; i >= 0; i--)
        {
            GameObject obj = trashCollected[i];

            if (obj == null)                     // destroyed
            {
                trashCollected.RemoveAt(i);
                continue;
            }

            if (!obj.activeInHierarchy)         // disabled
            {
                trashCollected.RemoveAt(i);
                continue;
            }

            // Check overlap with bounds
            Collider objCol = obj.GetComponent<Collider>();
            if (objCol == null || !myCollider.bounds.Intersects(objCol.bounds))
            {
                trashCollected.RemoveAt(i);
            }
        }
    }
    public int Collect()
    {
        int totalValue = 0;

        foreach (var trashGO in trashCollected)
        {
            int trashLayer = trashGO.layer;
            int acceptedLayer = LayerMask.NameToLayer(acceptedTrash.ToString());
            int price = 0;

            ItemPickup pickup = trashGO.GetComponent<ItemPickup>();
            if (pickup != null && pickup.itemDatabase != null)
            {
                ItemData data = pickup.itemDatabase.GetItemByID(pickup.itemID);
                if (data != null)
                    price = data.price;
            }

            if (trashLayer == acceptedLayer)
                totalValue += price;        // correct trash
            else
                totalValue -= price;        // wrong trash

            Destroy(trashGO);
        }

        trashCollected.Clear();
        return totalValue;
    }

    private void OnTriggerEnter(Collider other)
    {
        Trash trashObject = other.GetComponent<Trash>();
        if (trashObject != null)
            trashCollected.Add(other.gameObject);
    }

    //private void OnTriggerExit(Collider other)
    //{
    //    Trash trashObject = other.GetComponent<Trash>();
    //    if (trashObject != null)
    //        trashCollected.Remove(other.gameObject);
    //}
}
