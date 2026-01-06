using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    [Header("Spawn Settings")]
    public bool canSpawn = false;
    public float spawnCooldown = 10f;
    private float spawnTimer = 0f;

    [Header("Water Gauge")]
    public float waterGauge;
    public Slider waterSlider;
    [Tooltip("The text component for this gate (e.g. 'Water Level'). The Gauge Bar should be a child of this object.")]
    public TextMeshProUGUI waterText;
    private float maxWater = 100f;
    private float waterMultiplier = 0.5f;
    private float objectsMultiplier = 0f;

    [Header("Trash")]
    public ItemDatabaseSO itemDatabase;
    public List<ItemData> trashItems;
    public List<GameObject> trashObjects;
    public int objectsFound;
    public void ResetObjectsFound() => objectsFound = 0;

    [Header("Room Spawning")]
    RoomSpawner spawner;
    public Dictionary<Vector3, Room> spawnedRooms { get; private set; }

    // Gradual reduction
    private float targetWaterGauge;
    private float reduceSpeed = 0f;

    private void Awake()
    {
        // Filter and sort trash items
        trashItems = new List<ItemData>();
        if (itemDatabase != null)
        {
            foreach (var item in itemDatabase.GetItemsByType(ItemType.Trash))
            {
                if (item.itemType == ItemType.Trash)
                    trashItems.Add(item);
            }
        }

        spawner = GetComponent<RoomSpawner>();
    }

    private void Start()
    {
        InitializeClosedState();
    }

    public void SetSpawnedRooms(Dictionary<Vector3, Room> sharedSpawnedRooms)
    {
        spawnedRooms = sharedSpawnedRooms;
    }

    public void ClearSpawnedRooms()
    {
        spawnedRooms?.Clear();
    }

    private void Update()
    {
        if (FindAnyObjectByType<GameSystem>().dayStarted)
        {
            if (canSpawn)
            {
                HandleWaterGauge();
                HandleSpawnCooldown();
            }
        }
    }

    // --- UI & STATE MANAGEMENT ---

    /// <summary>
    /// Sets the gate to inactive, sets text to "Closed", and hides the gauge bar.
    /// </summary>
    public void InitializeClosedState()
    {
        canSpawn = false;

        if (waterText != null)
        {
            waterText.text = "Closed";

            // Assume the Gauge Bar is the first child of the Text object
            if (waterText.transform.childCount > 0)
            {
                waterText.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Activates the gate logic, enables the gauge bar, and spawns initial rooms.
    /// </summary>
    public void ActivateGate()
    {
        canSpawn = true;

        if (waterText != null && waterText.transform.childCount > 0)
        {
            waterText.transform.GetChild(0).gameObject.SetActive(true);
        }

        RespawnRooms();
        ResetObjectsFound();
    }

    public void ResetGateData()
    {
        waterGauge = 0f;
        targetWaterGauge = 0f;
        reduceSpeed = 0f;
        objectsFound = 0;
        spawnTimer = 0f;

        if (waterSlider != null)
            waterSlider.value = 0f;

        InitializeClosedState(); // Reset UI to closed
    }

    // ---------------------------

    private void HandleWaterGauge()
    {
        objectsMultiplier = objectsFound * 0.7f;
        float rate = waterMultiplier + objectsMultiplier;
        waterGauge += (Time.deltaTime / 10f) * rate;

        if (waterGauge > targetWaterGauge)
        {
            float step = reduceSpeed * Time.deltaTime;
            waterGauge = Mathf.Max(waterGauge - step, targetWaterGauge);
        }

        waterGauge = Mathf.Clamp(waterGauge, 0f, maxWater);

        if (waterSlider != null)
            waterSlider.value = waterGauge / maxWater;

        // Only update the number text if the gate is active (otherwise it stays "Closed")
        if (waterText != null && canSpawn)
            waterText.text = Mathf.FloorToInt(waterGauge).ToString();
    }

    private void HandleSpawnCooldown()
    {
        if (!canSpawn) return;

        spawnTimer -= Time.deltaTime;
        if (spawnTimer <= 0f)
        {
            spawnTimer = spawnCooldown;
            SpawnObjects();
        }
    }

    public void SpawnObjects()
    {
        if (spawnedRooms == null) return;

        List<Room> rooms = new List<Room>(spawnedRooms.Values);
        if (rooms.Count == 0) return;

        int roomsToSelect = Random.Range(1, 4);
        roomsToSelect = Mathf.Min(roomsToSelect, rooms.Count);

        for (int i = 0; i < roomsToSelect; i++)
        {
            Room room = rooms[i];
            if (room != null) room.SpawnGarbage();
        }
    }

    public void RespawnRooms()
    {
        foreach (GameObject trash in trashObjects)
        {
            if (trash != null)
                Destroy(trash);
        }

        trashObjects.Clear();
        if (spawner != null) spawner.RespawnRooms();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canSpawn)
        {
            if (waterSlider != null)
                waterSlider.gameObject.SetActive(true);
        }
    }

    public void ReduceWaterGradually(float amount, float duration)
    {
        if (duration <= 0f)
        {
            waterGauge = Mathf.Max(0f, waterGauge - amount);
            targetWaterGauge = waterGauge;
            reduceSpeed = 0f;
        }
        else
        {
            targetWaterGauge = Mathf.Max(0f, targetWaterGauge - amount);
            reduceSpeed = amount / duration;
        }
    }

    public void RemoveTrash(GameObject trashObject, float waterReduction, float duration)
    {
        if (trashObjects.Contains(trashObject))
            trashObjects.Remove(trashObject);

        ReduceWaterGradually(waterReduction, duration);
    }
}