using System.Collections.Generic;
using UnityEngine;
using static BuffType;

public class PlayerStatusEffects : MonoBehaviour
{
    private PlayerStats stats;
    private PlayerMovement movement;

    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    private float baseMaxHealth;
    private float baseMaxEnergy;
    private float baseSpeed;
    private float baseJump;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
        movement = GetComponent<PlayerMovement>();

        // Cache base values
        baseMaxHealth = stats.maxHealth;
        baseMaxEnergy = stats.maxEnergy;
        baseSpeed = movement.walkSpeed;
        baseJump = movement.jumpForce;
    }

    private void Update()
    {
        // Apply timed buffs
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];
            if (!effect.IsPermanent)
            {
                effect.elapsedTime += Time.deltaTime;
                if (effect.elapsedTime >= effect.duration)
                {
                    RemoveEffect(effect);
                    activeEffects.RemoveAt(i);
                }
            }
        }
    }

    public void ApplyEffect(StatusEffect effect)
    {
        Debug.Log($"Applying effect: {effect.buffType} Value: {effect.value} Duration: {effect.duration}");

        switch (effect.buffType)
        {
            case BuffType.RestoreHealth:
                stats.currentHealth = Mathf.Min(stats.currentHealth + effect.value, stats.maxHealth);
                stats.healthSlider.value = stats.currentHealth;
                return;

            case BuffType.RestoreEnergy:
                stats.currentEnergy = Mathf.Min(stats.currentEnergy + effect.value, stats.maxEnergy);
                stats.energySlider.value = stats.currentEnergy;
                return;

            case BuffType.IncreaseMaxHealth:
                stats.maxHealth += effect.value;
                stats.currentHealth += effect.value;
                stats.healthSlider.maxValue = stats.maxHealth;
                stats.healthSlider.value = stats.currentHealth;
                break;

            case BuffType.IncreaseMaxEnergy:
                stats.maxEnergy += effect.value;
                stats.currentEnergy += effect.value;
                stats.energySlider.maxValue = stats.maxEnergy;
                stats.energySlider.value = stats.currentEnergy;
                break;

            case BuffType.IncreaseSpeed:
                movement.walkSpeed += effect.value;
                break;

            case BuffType.IncreaseRunSpeed:
                movement.runSpeed += effect.value;
                break;

            case BuffType.IncreaseJumpHeight:
                movement.jumpForce += effect.value;
                break;

            //case BuffType.IncreaseHealthRegen:
            //    stats.energyRegenRate += effect.value; // Or create a new regen stat
            //    break;

            case BuffType.IncreaseEnergyRegen:
                stats.energyRegenRate += effect.value;
                break;
        }

        if (!effect.IsPermanent)
        {
            activeEffects.Add(effect);
        }
    }

    private void RemoveEffect(StatusEffect effect)
    {
        Debug.Log($"Removing effect: {effect.buffType}");

        switch (effect.buffType)
        {
            case BuffType.IncreaseMaxHealth:
                stats.maxHealth -= effect.value;
                stats.currentHealth = Mathf.Min(stats.currentHealth, stats.maxHealth);
                stats.healthSlider.maxValue = stats.maxHealth;
                stats.healthSlider.value = stats.currentHealth;
                break;

            case BuffType.IncreaseMaxEnergy:
                stats.maxEnergy -= effect.value;
                stats.currentEnergy = Mathf.Min(stats.currentEnergy, stats.maxEnergy);
                stats.energySlider.maxValue = stats.maxEnergy;
                stats.energySlider.value = stats.currentEnergy;
                break;

            case BuffType.IncreaseSpeed:
                movement.walkSpeed -= effect.value;
                break;

            case BuffType.IncreaseRunSpeed:
                movement.runSpeed -= effect.value;
                break;

            case BuffType.IncreaseJumpHeight:
                movement.jumpForce -= effect.value;
                break;

            case BuffType.IncreaseHealthRegen:
                stats.energyRegenRate -= effect.value;
                break;

            case BuffType.IncreaseEnergyRegen:
                stats.energyRegenRate -= effect.value;
                break;
        }
    }
}
