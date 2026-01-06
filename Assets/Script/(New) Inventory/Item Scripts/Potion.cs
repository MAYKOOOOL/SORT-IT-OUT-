using UnityEngine;

public enum PotionType
{
    RestoreHealth,
    RestoreEnergy,
    IncreaseEnergyRegen,
    IncreaseJump,
    IncreaseMaxEnergy,
    IncreaseMaxHealth,
    ThrowStrength,
    RunSpeed,
}

public class Potion : Consumable
{
    [Header("Potion Settings")]
    public PotionType potionType;
    public float amount;

    private PlayerStats stats;
    private PlayerMovement movement;

    [Header("Buff UI")]
    [SerializeField] private BuffSlot buffSlot; // Assign in Inspector or find in scene

    private void Awake()
    {
        // Get player components
        stats = FindAnyObjectByType<PlayerStats>();
        movement = FindAnyObjectByType<PlayerMovement>();

        // Optionally find BuffSlot if not assigned
        if (buffSlot == null)
        {
            buffSlot = FindAnyObjectByType<BuffSlot>();
            if (buffSlot == null)
                Debug.LogWarning("BuffSlot not found in scene!");
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    public override void UseItem()
    {
        if (stats == null || movement == null) return;

        // Play particle effect from base
        base.UseItem();

        bool hasBuffIcon = false;
        int buffIndex = -1;
        string announcementMessage = "";

        // Apply potion effects
        switch (potionType)
        {
            case PotionType.RestoreHealth:
                stats.RestoreHealth(amount);
                announcementMessage = "Health Restored!";
                break;

            case PotionType.RestoreEnergy:
                stats.RestoreEnergy(amount);
                announcementMessage = "Energy Restored!";
                break;

            case PotionType.IncreaseEnergyRegen:
                stats.IncreaseEnergyRegen(amount);
                hasBuffIcon = true;
                buffIndex = 0;
                announcementMessage = "Energy Regen Level Up!";
                break;

            case PotionType.IncreaseJump:
                movement.permanentJumpBoost += amount;
                hasBuffIcon = true;
                buffIndex = 1;
                announcementMessage = "Jump Boost Level Up!";
                break;

            case PotionType.IncreaseMaxEnergy:
                stats.IncreaseMaxEnergy(amount);
                hasBuffIcon = true;
                buffIndex = 2;
                announcementMessage = "Max Energy Increased!";
                break;

            case PotionType.IncreaseMaxHealth:
                stats.IncreaseMaxHealth(amount);
                hasBuffIcon = true;
                buffIndex = 3;
                announcementMessage = "Max Health Increased!";
                break;

            case PotionType.ThrowStrength:
                movement.permanentThrowBoost += amount;
                hasBuffIcon = true;
                buffIndex = 4;
                announcementMessage = "Throw Strength Level Up!";
                break;

            case PotionType.RunSpeed:
                movement.permanentSpeedBoost += amount;
                hasBuffIcon = true;
                buffIndex = 5;
                announcementMessage = "Run Speed Level Up!";
                break;
        }

        // Add buff icon if applicable
        if (hasBuffIcon && buffSlot != null)
        {
            buffSlot.AddBuff(buffIndex);
        }

        if (Announcement.Instance != null)
        {
            Announcement.Instance.SetAnnouncement(announcementMessage);
        }
        // Destroy the potion object after use
        Destroy(gameObject);

        Debug.Log($"Used potion: {potionType}");
    }
}