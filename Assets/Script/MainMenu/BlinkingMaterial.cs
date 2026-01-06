using UnityEngine;
using System.Collections; // Needed for IEnumerator

public class BlinkingMaterial : MonoBehaviour
{
    [Header("Blinking Settings")]
    [Tooltip("The time in seconds the material is 'on' (glowing).")]
    public float blinkOnDuration = 0.8f;
    [Tooltip("The time in seconds the material is 'off' (dark).")]
    public float blinkOffDuration = 0.2f;

    [Tooltip("The intensity of the emission when 'on'.")]
    public float emissionIntensity = 5f;
    [Tooltip("The base color of the emission when 'on'.")]
    public Color emissionColor = Color.cyan;

    private Renderer objectRenderer;
    private Material instanceMaterial; // We'll work with an instance
    private Color originalEmissionColor; // To store the starting emission color if any
    private bool isBlinking = false;

    // Shader property ID for emission color
    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    void Awake()
    {
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component not found on " + gameObject.name + "! Cannot blink material.", this);
            enabled = false;
            return;
        }

        // Get an instance of the material to avoid modifying the project asset directly
        instanceMaterial = objectRenderer.material;

        // Store original emission color, or default to black if none
        originalEmissionColor = instanceMaterial.GetColor(EmissionColorID);
        if (originalEmissionColor == Color.black)
        {
            // If emission was off, ensure we have a starting point based on the desired color
            originalEmissionColor = emissionColor;
        }
    }

    void OnEnable()
    {
        // Start blinking when the GameObject becomes active
        if (!isBlinking)
        {
            StartCoroutine(BlinkRoutine());
        }
    }

    void OnDisable()
    {
        // Stop blinking when the GameObject becomes inactive
        if (isBlinking)
        {
            StopCoroutine(BlinkRoutine());
            isBlinking = false;
            // Ensure the light is left in an 'off' state or original state when disabled
            SetEmission(false);
        }
    }

    private IEnumerator BlinkRoutine()
    {
        isBlinking = true;
        while (isBlinking) // Loop indefinitely while blinking is active
        {
            // Turn on emission
            SetEmission(true);
            yield return new WaitForSeconds(blinkOnDuration);

            // Turn off emission
            SetEmission(false);
            yield return new WaitForSeconds(blinkOffDuration);
        }
    }

    private void SetEmission(bool on)
    {
        if (instanceMaterial == null) return;

        if (on)
        {
            // Set emission color with desired intensity
            instanceMaterial.SetColor(EmissionColorID, emissionColor * emissionIntensity);
            // Enable emission keyword for the material (if not already enabled)
            instanceMaterial.EnableKeyword("_EMISSION");
        }
        else
        {
            // Turn off emission by setting it to black (zero intensity)
            instanceMaterial.SetColor(EmissionColorID, Color.black);
            // Optionally disable keyword to save a tiny bit of performance if material allows
            // instanceMaterial.DisableKeyword("_EMISSION"); 
        }
    }

    void OnDestroy()
    {
        // Clean up the instantiated material instance when the object is destroyed
        if (instanceMaterial != null)
        {
            Destroy(instanceMaterial);
        }
    }
}