using UnityEngine;

public class GroundItem : MonoBehaviour
{
    public Pedestal parentPedestal;
    public bool purchased = false;

    public void Purchase()
    {
        purchased = true;
        parentPedestal.MarkPurchased();
    }
}
