using UnityEngine;

public class trig_col : MonoBehaviour
{
    [Header("player")]
    public PlayerStats stats;
    public ipis ipis;

    void Start()
    {
        ipis = GetComponentInParent<ipis>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stats = other.gameObject.GetComponent<PlayerStats>();
            ipis.player = other.transform;  // Assign player
            ipis.playerDetected = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ipis.playerDetected = false;
            ipis.player = null;
        }
    }
}
