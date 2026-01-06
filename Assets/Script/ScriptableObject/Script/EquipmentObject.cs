using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment Ojbect", menuName = "Inventory System/Items/Equipment")]
public class EquipmentObject : ItemObject
{
    public List<EquipmentEffect> equipmentEffects = new List<EquipmentEffect>();

    private void Awake()
    {
        type = ItemType.Equipment;
    }

    public override void OnUse()
    {
        PlayerEquipmentEffects playerEffects = FindFirstObjectByType<PlayerEquipmentEffects>();
        if (playerEffects == null)
        {
            Debug.LogWarning("PlayerEquipmentEffects not found in scene.");
            return;
        }

        foreach (var effect in equipmentEffects)
        {
            playerEffects.UseEquipment(effect);
        }
    }
}
