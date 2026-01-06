using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container = new Inventory();

    public InventorySlot AddItem(Item _item, int _amount, GameObject linkedObject)
    {
        return SetEmptySlot(_item, _amount, linkedObject);
    }

    public InventorySlot SetEmptySlot(Item _item, int _amount, GameObject linkedObject)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].ID <= -1)
            {
                Container.Items[i].linkedObject = linkedObject;
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i];
            }
        }
        return null;
    }

    public void MoveItem(InventorySlot item, InventorySlot item2)
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount, item2.linkedObject);
        item2.UpdateSlot(item.ID, item.item, item.amount, item.linkedObject);
        item.UpdateSlot(temp.ID, temp.item, temp.amount, temp.linkedObject);
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                if (Container.Items[i].linkedObject != null)
                {
                    GameObject.Destroy(Container.Items[i].linkedObject);
                    Container.Items[i].linkedObject = null;
                }

                Container.Items[i].UpdateSlot(-1, null, 0, null);
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }

    [ContextMenu("Load")]
    public void Load()
    {
        if (File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Inventory newContainer = (Inventory)formatter.Deserialize(stream);
            for (int i = 0; i < Container.Items.Length; i++)
            {
                Container.Items[i].UpdateSlot(newContainer.Items[i].ID, newContainer.Items[i].item, newContainer.Items[i].amount, null); // reassign GameObject after load
            }
            stream.Close();
        }
    }

    [ContextMenu("Clear")]
    public void Clear()
    {
        foreach (var slot in Container.Items)
        {
            if (slot.linkedObject != null)
            {
                GameObject.Destroy(slot.linkedObject);
                slot.linkedObject = null;
            }

            slot.UpdateSlot(-1, null, 0, null);
        }
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[9];
}

[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];

    public UserInterface parent;
    public int ID = -1;
    public Item item;
    public int amount;

    public GameObject linkedObject;

    public InventorySlot()
    {
        ID = -1;
        item = null;
        amount = 0;
        linkedObject = null;
    }

    public InventorySlot(int _id, Item _item, int _amount, GameObject _linkedObject = null)
    {
        ID = _id;
        item = _item;
        amount = _amount;
        linkedObject = _linkedObject;
    }

    public void UpdateSlot(int _id, Item _item, int _amount, GameObject _linkedObject = null)
    {
        ID = _id;
        item = _item;
        amount = _amount;
        linkedObject = _linkedObject;
    }
        
    public bool CanPlaceInSlot(ItemObject _item)
    {
        if (AllowedItems.Length <= 0)
            return true;

        for (int i = 0; i < AllowedItems.Length; i++)
        {
            if (_item.type == AllowedItems[i])
            {
                return true;
            }
        }
        return false;
    }
}
