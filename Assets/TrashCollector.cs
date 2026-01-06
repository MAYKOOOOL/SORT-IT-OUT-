using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class TrashCollector : MonoBehaviour
{
    public Teleport trashTP;
    public bool isTrash = true;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Trash trash = other.GetComponentInParent<Trash>();
        if (trash != null)
        {
            // Teleport the trash
            if (trashTP != null)
                trashTP.TeleportPlayer(trash.gameObject);
        }
    }
}
