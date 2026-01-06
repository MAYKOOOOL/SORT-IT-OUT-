using UnityEngine;

public class Coin : Consumable
{
    public int value = 0;

    // Helper to ensure we don't double-trigger if spam clicked before destroy
    private bool isUsed = false;

    public override void UseItem()
    {
        if (isUsed) return;
        isUsed = true;

        // 1. Notify the Tutorial script (if it exists in the scene)
        StoreTutorial tutorial = FindAnyObjectByType<StoreTutorial>();
        if (tutorial != null)
        {
            tutorial.ReportCoinConsumed();
        }

        // 2. Add Coins
        if (playerStats != null)
        {
            playerStats.UpdateCoins(value);
        }

        // 3. Destroy
        Destroy(gameObject);
    }
}