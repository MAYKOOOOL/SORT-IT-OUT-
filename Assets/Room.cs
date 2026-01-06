using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    private List<ItemData> trashItems = new List<ItemData>();

    public Transform northTunnel;
    public Transform southTunnel;
    public Transform eastTunnel;
    public Transform westTunnel;

    public Gate gate;
    public GameObject water;
    public Transform trashSpawn;

    private float minY = -17f;
    private float maxY = 0f;
    private void Awake()
    {
        
    }

    void Start()
    {
        if(Random.Range(0,3) == 0)
        {
            SpawnGarbage();
            Debug.Log("AAAAAAAAAAAAAAAAAA");
        }
    }

    void Update()
    {
        UpdateWaterPosition();
    }

    private void UpdateWaterPosition()
    {
        if (gate == null || water == null) return;

        float normalizedWater = Mathf.Clamp01(gate.waterGauge / 100f); // Normalize between 0 and 1
        float newY = Mathf.Lerp(minY, maxY, normalizedWater); // Interpolate between -17 and 0

        Vector3 pos = water.transform.localPosition;
        pos.y = newY;
        water.transform.localPosition = pos;
    }

    public void SetGate(Gate gate)
    {
        this.gate = gate;
        trashItems = gate.trashItems;
    }

    public void SpawnGarbage()
    {
        GameObject spawnedItem = Instantiate(trashItems[Random.Range(0, trashItems.Count)].prefab, trashSpawn.position, Quaternion.identity);
        Trash trashScript = spawnedItem.GetComponent<Trash>();
        trashScript.linkedGate = gate;

        Rigidbody rb = spawnedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Rotate randomly around Y axis
            float randomYRotation = Random.Range(0f, 360f);
            spawnedItem.transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);

            // Apply forward force based on the new rotation
            Vector3 forwardForce = spawnedItem.transform.forward * Random.Range(2f, 5f); // force in forward direction
            forwardForce.y = Random.Range(2f, 5f); // upward force

            rb.AddForce(forwardForce, ForceMode.Impulse);
        }

        gate.objectsFound += 1;
        gate.trashObjects.Add(spawnedItem);
    }
}
