using UnityEngine;

[System.Serializable]
public class EquipmentEffect
{
    public EquipmentType equipmentType;

    public EquipmentEffect(EquipmentType type)
    {
        this.equipmentType = type;
    }
}
