using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ItemPickup : MonoBehaviour
{
    [Header("Item Info (Assigned Automatically or in Inspector)")]
    public ItemDatabaseSO itemDatabase;
    [SerializeField]public int itemID = -1;
}
