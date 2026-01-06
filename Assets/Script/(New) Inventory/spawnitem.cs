using UnityEngine;

public class SpawnItem : MonoBehaviour
{
    public InventorySO inventory; // Reference to the InventorySO scriptable object

    void Start()
    {
        inventory.SpawnWorldItemByID(0, transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
