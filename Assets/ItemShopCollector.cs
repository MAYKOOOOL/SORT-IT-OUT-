using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ItemShopCollector : MonoBehaviour
{
    public List<GameObject> itemsCollected = new List<GameObject>();
    public int totalPrice = 0;
    public TextMeshProUGUI priceText;

    private Collider myTrigger;

    private void Start()
    {
        myTrigger = GetComponent<Collider>();
    }

    private void Update()
    {
        CleanupItems();
    }

    private void CleanupItems()
    {
        for (int i = itemsCollected.Count - 1; i >= 0; i--)
        {
            GameObject obj = itemsCollected[i];

            // 1. Destroyed
            if (obj == null)
            {
                itemsCollected.RemoveAt(i);
                continue;
            }

            // 2. Disabled
            if (!obj.activeInHierarchy)
            {
                itemsCollected.RemoveAt(i);
                continue;
            }

            // 3. No collider?
            Collider col = obj.GetComponent<Collider>();
            if (col == null)
            {
                itemsCollected.RemoveAt(i);
                continue;
            }

            // 4. Bounds do not touch => outside trigger
            if (!myTrigger.bounds.Intersects(col.bounds))
            {
                itemsCollected.RemoveAt(i);
            }
        }

        TotalPrice();
    }

    public void Purchase()
    {
        foreach (GameObject item in itemsCollected)
        {
            GroundItem groundItem = item.GetComponent<GroundItem>();
            groundItem.Purchase();
        }

        itemsCollected.Clear();
        TotalPrice();
    }

    public void TotalPrice()
    {
        totalPrice = 0;

        foreach (GameObject item in itemsCollected)
        {
            GroundItem gi = item.GetComponent<GroundItem>();
            totalPrice += gi.parentPedestal.itemData.price;
        }

        priceText.text = $"{totalPrice}G";
    }

    private void OnTriggerEnter(Collider other)
    {
        GroundItem groundItem = other.gameObject.GetComponent<GroundItem>();
        if (groundItem != null && !groundItem.purchased)
        {
            if (!itemsCollected.Contains(other.gameObject))
                itemsCollected.Add(other.gameObject);
        }

        TotalPrice();
    }
}
