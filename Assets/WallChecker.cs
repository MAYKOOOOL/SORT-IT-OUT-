using Unity.VisualScripting;
using UnityEngine;

public class WallChecker : MonoBehaviour
{
    [HideInInspector] public bool isTouchingWall = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            isTouchingWall = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            isTouchingWall = false;
        }
    }
}
