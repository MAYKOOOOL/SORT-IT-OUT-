using UnityEngine;

public class Ammo : Consumable
{
    public int ammoAmount = 10;

    public override void UseItem()
    {
        base.UseItem();
        if (playerStats == null) return;

        playerStats.UpdateBullets(ammoAmount);


        Destroy(gameObject);
        Debug.Log("Ammo item used and removed from inventory.");
    }
}