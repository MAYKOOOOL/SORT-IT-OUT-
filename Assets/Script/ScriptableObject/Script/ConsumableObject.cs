using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Ojbect", menuName = "Inventory System/Items/Consumable")]
public class ConsumableObject : ItemObject
{
    public List<StatusEffect> effects = new List<StatusEffect>();

    private void Awake()
    {
        type = ItemType.Consumable;
    }

    public override void OnUse()
    {
        var player = GameObject.FindWithTag("Player");
        if (player == null) return;

        var statusSystem = player.GetComponent<PlayerStatusEffects>();
        if (statusSystem == null) return;

        foreach (var effect in effects)
        {
            statusSystem.ApplyEffect(new StatusEffect(effect.buffType, effect.value, effect.duration));
        }
    }
}
