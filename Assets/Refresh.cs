using Unity.VisualScripting;
using UnityEngine;

public class Refresh : MonoBehaviour
{
    public Shop shop;
    bool once = false;
    bool isPaused = false;
    public int coinReduce;
    public bool reset = false;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            if(shop != null)
            {
                if (!reset)
                {
                    shop.RefreshShop();
                    FindAnyObjectByType<PlayerStats>().UpdateCoins(-coinReduce);
                }
                else
                {
                    shop.ResetShop();
                }
            }
            
        }
    }
}
