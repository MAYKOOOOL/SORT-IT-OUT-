using UnityEngine;


[System.Serializable]
public class ItemData
{
    public int id = -1;              // -1 means empty
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public GameObject prefab;        // Optional, for spawning
    public int price;
}