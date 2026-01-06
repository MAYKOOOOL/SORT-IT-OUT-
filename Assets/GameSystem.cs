using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameSystem : MonoBehaviour
{
    [Header("References")]
    public GameObject player;
    public List<Gate> gates;
    public Shop shop;

    [Header("Day Settings")]
    public float dayTimer = 0f;
    public int day = 0;
    public bool dayStarted = false;

    [Header("Gate Timer Settings")]
    private float gateCheckTimer = 0f;
    private float minGateCheckInterval = 15f;  // random between 15–40 seconds
    private float maxGateCheckInterval = 40f;
    private List<Gate> gatesNotActivated;

    [Header("UI")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI dayText;
    // Removed gateWaterText array as Gates now handle their own UI references
    public GameObject waterGauge;

    [Header("Navigation")]
    public GameObject compass;
    public GameObject target;
    private Quaternion compassDefaultRotation;

    [Header("Teleports")]
    public Teleport extract;
    public Teleport trash;

    private void Start()
    {
        if (compass != null)
            compassDefaultRotation = compass.transform.localRotation;
    }

    private void Update()
    {
        if (dayStarted)
        {
            // 1. Day Timer Logic
            dayTimer -= Time.deltaTime;

            if (timerText != null)
                timerText.text = Mathf.CeilToInt(dayTimer).ToString() + "s";

            // End Day Check
            if (dayTimer <= 0f)
            {
                EndDay();
                return;
            }

            // 2. Gate Activation Logic
            gateCheckTimer -= Time.deltaTime;

            if (gateCheckTimer <= 0f)
            {
                ActivateRandomGate();
                gateCheckTimer = Random.Range(minGateCheckInterval, maxGateCheckInterval);
            }
        }
    }

    private void LateUpdate()
    {
        if (dayStarted && compass != null && target != null && player != null)
            UpdateCompass();
    }

    public void StartDay()
    {
        dayStarted = true;
        day++;

        if (dayText != null)
            dayText.text = "Day: " + day.ToString();

        // Reset Day Timer (e.g. 2 minutes)
        dayTimer = 120f;

        // 1. Regenerate rooms geometry (Keep gates closed logically)
        if (day > 1)
        {
            RegenerateAllRooms();
        }

        shop.ResetShop();

        // 2. Initialize Gates (Set all to "Closed" visually and logically)
        gatesNotActivated = new List<Gate>(gates);

        foreach (var gate in gates)
        {
            gate.InitializeClosedState();
        }

        // 3. Immediately activate the first gate
        ActivateRandomGate();

        // Set timer for the next gate
        gateCheckTimer = Random.Range(minGateCheckInterval, maxGateCheckInterval);

        Debug.Log($"New Day {day} started. First gate activated.");
    }

    private void EndDay()
    {
        dayStarted = false;

        // Reset all gates data and UI
        foreach (var gate in gates)
        {
            gate.ResetGateData();
        }

        if (waterGauge != null)
            waterGauge.SetActive(false);

        if (compass != null)
            compass.transform.localRotation = compassDefaultRotation;

        Extract();

        Debug.Log("Day ended. Gates disabled, gauges reset.");
    }

    private void RegenerateAllRooms()
    {
        foreach (var gate in gates)
        {
            // Respawn geometry but keep gate logic OFF
            gate.RespawnRooms();
            gate.ResetObjectsFound();
            // Ensure logic remains off until ActivateRandomGate calls ActivateGate()
            gate.canSpawn = false;
        }
        Debug.Log("All rooms regenerated for new day.");
    }

    private void ActivateRandomGate()
    {
        if (gatesNotActivated == null || gatesNotActivated.Count == 0)
            return; // No gates left to open

        // 1. Pick a random closed gate
        Gate chosenGate = gatesNotActivated[Random.Range(0, gatesNotActivated.Count)];

        // 2. Activate it (Enables UI Gauge, sets bools, starts spawning)
        chosenGate.ActivateGate();

        // 3. Remove from available list
        gatesNotActivated.Remove(chosenGate);

        Debug.Log("Gate activated: " + chosenGate.name);
    }

    private void UpdateCompass()
    {
        Vector3 direction = target.transform.position - player.transform.position;
        direction.y = 0f;

        float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        compass.transform.localRotation = Quaternion.Euler(0f, 0f, -angle);
    }

    public void Extract()
    {
        if (extract != null)
        {
            extract.animator.SetTrigger("Transition");
            extract.DelayedTeleportPlayer();
        }
    }
}