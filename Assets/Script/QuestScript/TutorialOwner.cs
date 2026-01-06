using UnityEngine;

public class TutorialOwner : MonoBehaviour
{
    public GameObject owner;
    public Transform spawnPosition;

    private void OnTriggerEnter(Collider other)
    {
        if (owner != null)
        {
            owner.transform.position = new Vector3(spawnPosition.position.x, owner.transform.position.y, spawnPosition.position.z);
        }
    }
}
