using UnityEngine;

public class CreditsButtonTrigger : MonoBehaviour
{
    // --- References ---
    private MainMenuManager menuManager;
    private Renderer objectRenderer; // Needed for material swap

    // --- Configuration for Hover Materials ---
    [Header("Hover Material")]
    [Tooltip("The material to use when the mouse is hovering over the object.")]
    public Material hoverMaterial;
    private Material originalMaterial;

    // --- Configuration for Scale Animation ---
    [Header("Hover Scale Animation")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;
    private Vector3 originalScale;
    private Vector3 targetScale; // Target for smooth scaling

    // --- Configuration for Press Animation ---
    [Header("Press Animation")]
    [Tooltip("The local scale multiplier when the button is actively being pressed down.")]
    public float pressedScaleFactor = 0.9f;
    private Vector3 pressedScale;

    void Awake()
    {
        // 1. Get the Renderer component
        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer == null)
        {
            Debug.LogError("Renderer component not found on " + gameObject.name + "! Cannot swap material.");
        }
        else
        {
            // 2. Store the original material
            // NOTE: You must use .sharedMaterial if your object uses the same material instance 
            // across multiple objects to avoid creating unnecessary material copies.
            originalMaterial = objectRenderer.sharedMaterial;
        }

        // 3. Initialize Scale Variables
        originalScale = transform.localScale;
        targetScale = originalScale;
        pressedScale = originalScale * pressedScaleFactor;
    }

    void Start()
    {
        // Find the MainMenuManager instance
        GameObject main = GameObject.FindGameObjectWithTag("Manager");

        if (main != null)
        {
            menuManager = main.GetComponent<MainMenuManager>();

            if (menuManager == null)
                Debug.LogError("MainMenuManager component missing on object with tag 'Manager'!");
        }
    }

    void Update()
    {
        // Smoothly transition the object's scale
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleSpeed);
    }

    // ------------------------------------
    // --- Mouse Events for Hover & Click ---
    // ------------------------------------

    private void OnMouseEnter()
    {
        Debug.Log("Credits Button: Mouse Enter (Hover)");
        // Swap to the hover material
        if (objectRenderer != null && hoverMaterial != null)
        {
            // Use .material to swap material on this instance only
            objectRenderer.material = hoverMaterial;
        }

        // Set the target scale (for smooth scale-up)
        targetScale = originalScale * hoverScale;
    }

    private void OnMouseExit()
    {
        Debug.Log("Credits Button: Mouse Exit (Unhover)");
        // Swap back to the original material
        if (objectRenderer != null && originalMaterial != null)
        {
            // Use .material to swap material on this instance only
            objectRenderer.material = originalMaterial;
        }

        // Set the target scale back to original (for smooth scale-down)
        targetScale = originalScale;
    }

    private void OnMouseDown()
    {
        Debug.Log("Credits Button: Pressing Down");
        // Immediately set the scale to the smaller, 'pressed' size
        targetScale = pressedScale;
    }

    private void OnMouseUp()
    {
        Debug.Log("Credits Button: Releasing Up");
        // Restore the scale to the appropriate state (hovered or original)
        // Check if the material is currently the hover material to know if the mouse is still over the button
        if (objectRenderer != null && objectRenderer.material == hoverMaterial)
        {
            // If still hovering, return to the hovered scale
            targetScale = originalScale * hoverScale;
        }
        else
        {
            // If mouse exited while pressed, return to the original scale
            targetScale = originalScale;
        }
    }

    // --- Credits Display Trigger ---

    private void OnMouseUpAsButton()
    {
        // This event fires if OnMouseDown and OnMouseUp occurred over the same collider
        if (menuManager != null)
        {
            Debug.Log($"Mouse released over {gameObject.name}. Triggering Credits Display.");

            // Final visual cleanup before the panel opens
            transform.localScale = originalScale;

            // CORRECTED: Call the ShowCredits method in the MainMenuManager
            menuManager.ShowCredits();
        }
    }
}