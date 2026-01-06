using UnityEngine;

public class WaterRippleEffect : MonoBehaviour
{
    // --- Configuration ---
    [Header("Simulation Settings")]
    [Tooltip("The size of the grid (e.g., 64x64 or 128x128). Must be a power of 2 for potential optimization.")]
    public int resolution = 64;
    [Tooltip("The speed at which the ripple propagates.")]
    public float waveSpeed = 1.0f;
    [Tooltip("The factor by which the wave height decays (energy loss).")]
    public float dampening = 0.99f;
    [Tooltip("Multiplier for the height of the initial splash.")]
    public float splashForce = 0.5f;

    [Header("References")]
    [Tooltip("The Transform of the object causing ripples (e.g., the player).")]
    public Transform rippleSource;

    // --- Internal State ---
    private float[,] currentHeight;
    private float[,] previousHeight;
    private Mesh mesh;
    private Vector3[] vertices;

    private float timer;
    private const float FixedTimeStep = 0.015f; // Simulation runs at ~66 FPS

    void Start()
    {
        SetupWaterMesh();
        InitializeHeightMaps();

        // Find the player object to use as the ripple source
        if (rippleSource == null)
        {
            // Assuming the PlayerController handles the player's main transform
            if (PlayerController.Instance != null)
            {
                rippleSource = PlayerController.Instance.transform;
            }
            else
            {
                Debug.LogError("Ripple source (Player) is not assigned. Ripples won't be created.");
            }
        }
    }

    // Runs simulation at a fixed rate, independent of frame rate
    void FixedUpdate()
    {
        timer += Time.deltaTime;

        // Only run the simulation when the player is in the water
        if (PlayerController.Instance != null && PlayerController.Instance.onSwim)
        {
            ApplyRippleSource();
        }

        // Run the simulation steps until the timer is consumed
        while (timer >= FixedTimeStep)
        {
            SimulateRipples();
            UpdateMeshVertices();
            timer -= FixedTimeStep;
        }
    }

    // --- Simulation Core Functions ---

    private void SetupWaterMesh()
    {
        // Get the MeshFilter and Mesh
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Water object needs a MeshFilter component.");
            enabled = false;
            return;
        }

        mesh = meshFilter.mesh;
        vertices = mesh.vertices;

        // Ensure the mesh has the right number of vertices for the chosen resolution
        if (vertices.Length != resolution * resolution)
        {
            Debug.LogWarning($"Mesh vertex count ({vertices.Length}) does not match resolution squared ({resolution * resolution}). Ripples may look distorted.");
        }
    }

    private void InitializeHeightMaps()
    {
        currentHeight = new float[resolution, resolution];
        previousHeight = new float[resolution, resolution];
    }

    private void ApplyRippleSource()
    {
        if (rippleSource == null) return;

        // Convert player's world position to a point on the water's local grid
        Vector3 localPos = transform.InverseTransformPoint(rippleSource.position);

        // Normalize local position from -MeshSize/2 to +MeshSize/2 to 0 to Resolution-1
        // We use the water object's scale to determine the mesh size
        float scaleX = transform.localScale.x;
        float scaleZ = transform.localScale.z;

        // Simple mapping: [ -scale/2, scale/2 ] -> [ 0, resolution-1 ]
        int x = Mathf.Clamp(Mathf.RoundToInt((localPos.x / scaleX + 0.5f) * (resolution - 1)), 1, resolution - 2);
        int z = Mathf.Clamp(Mathf.RoundToInt((localPos.z / scaleZ + 0.5f) * (resolution - 1)), 1, resolution - 2);

        // Apply a small ripple force at the calculated point
        currentHeight[x, z] += splashForce;
    }

    private void SimulateRipples()
    {
        // Create a temporary height map for the next state
        float[,] nextHeight = new float[resolution, resolution];

        // Loop through all points inside the border
        for (int x = 1; x < resolution - 1; x++)
        {
            for (int z = 1; z < resolution - 1; z++)
            {
                // The core 2D Wave Equation (Finite Difference Method):
                // Next height = Average of neighbors / 2 - Previous height * Dampening
                float neighborsSum = currentHeight[x - 1, z] + currentHeight[x + 1, z] +
                                     currentHeight[x, z - 1] + currentHeight[x, z + 1];

                nextHeight[x, z] = (neighborsSum / 2.0f) - previousHeight[x, z];
                nextHeight[x, z] *= dampening;
            }
        }

        // Swap the height maps for the next iteration (t-1 becomes t, t becomes t+1)
        previousHeight = currentHeight;
        currentHeight = nextHeight;
    }

    private void UpdateMeshVertices()
    {
        // Transfer the height map data to the mesh vertices
        for (int x = 0; x < resolution; x++)
        {
            for (int z = 0; z < resolution; z++)
            {
                // Map the 2D array index (x, z) to the 1D vertex array index (i)
                int i = z * resolution + x;

                // Set the vertex Y-position (height) based on the current height map
                vertices[i].y = currentHeight[x, z] * waveSpeed;
            }
        }

        // Apply the modified vertices back to the mesh
        mesh.vertices = vertices;
        // Recalculate normals so lighting looks correct on the displaced surface
        mesh.RecalculateNormals();
    }

    void OnDestroy()
    {
        // Clean up any instantiated mesh or material if necessary, though 
        // this script primarily manipulates the existing mesh.
    }
}