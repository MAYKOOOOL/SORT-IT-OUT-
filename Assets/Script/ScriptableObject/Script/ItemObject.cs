using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Consumable,
    Equipment,
    Trash
}


public class ItemObject : ScriptableObject
{
    public int Id;
    public Sprite uiDisplay;
    public GameObject objectDisplay;
    public GameObject prefab;
    public ItemType type;
    public int price = 0;
    [TextArea (15,20)]
    public string description;

    public virtual void OnUse() 
    {
        
    }
}

[System.Serializable]
public class Item
{
    public string Name;
    public int Id;
    public Item(ItemObject item)
    {
        Name = item.name;
        Id = item.Id;
    }
}