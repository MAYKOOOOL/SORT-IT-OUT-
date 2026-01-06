using UnityEngine;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour
{
    public ItemType spawnerItemType;

    [Header("Items")]
    public List<ItemData> shopItems = new List<ItemData>(); // assigned by Shop
    public List<ItemData> toSpawn = new List<ItemData>();

    [Header("Pedestals")]
    public List<Transform> pedestalTransforms;

    public void ResetPedestals()
    {
        foreach (var t in pedestalTransforms)
        {
            var ped = t.GetComponent<Pedestal>();
            if (ped != null)
            {
                ped.ClearPedestal();
            }
        }
        SpawnItems();
    }

    public void RefreshNewSet()
    {
        PrepareNewItems();
        SpawnItems();
    }

    private void PrepareNewItems()
    {
        toSpawn.Clear();

        if (shopItems.Count == 0) return;

        foreach (var t in pedestalTransforms)
        {
            ItemData randomItem = shopItems[Random.Range(0, shopItems.Count)];
            toSpawn.Add(randomItem);
        }
    }

    private void SpawnItems()
    {
        if (toSpawn.Count == 0)
        {
            Debug.LogWarning($"Spawner '{name}' has no items to spawn.");
            return;  // prevent crash
        }

        int count = Mathf.Min(toSpawn.Count, pedestalTransforms.Count);

        for (int i = 0; i < count; i++)
        {
            Pedestal ped = pedestalTransforms[i].GetComponent<Pedestal>();
            if (ped == null) continue;

            ped.SpawnItem(toSpawn[i]);
        }
    }
}
