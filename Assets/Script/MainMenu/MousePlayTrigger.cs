using UnityEngine;

public class MousePlayTrigger : MonoBehaviour
{
    private MainMenuManager menuManager;
    private Renderer objectRenderer;

    [Header("Hover Material")]
    public Material hoverMaterial;
    private Material originalMaterial;

    [Header("Hover Scale Animation")]
    public float hoverScale = 1.1f;
    public float scaleSpeed = 10f;
    private Vector3 originalScale;
    private Vector3 targetScale;

    [Header("Press Animation")]
    public float pressedScaleFactor = 0.9f;
    private Vector3 pressedScale;

    void Awake()
    {
        // Force time to run so animations work when returning to menu
        Time.timeScale = 1f;

        objectRenderer = GetComponent<Renderer>();
        if (objectRenderer != null)
        {
            // Use sharedMaterial to avoid creating material instances every time
            originalMaterial = objectRenderer.sharedMaterial;
        }

        originalScale = transform.localScale;
        targetScale = originalScale;
        pressedScale = originalScale * pressedScaleFactor;
    }

    void Start()
    {
        FindManager();
    }

    private void FindManager()
    {
        GameObject main = GameObject.FindGameObjectWithTag("Manager");
        if (main != null)
        {
            menuManager = main.GetComponent<MainMenuManager>();
        }
    }

    void Update()
    {
        // Use unscaledDeltaTime so the button works even if Time.timeScale is 0
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    private void OnMouseEnter()
    {
        if (hoverMaterial != null) objectRenderer.material = hoverMaterial;
        targetScale = originalScale * hoverScale;
    }

    private void OnMouseExit()
    {
        objectRenderer.material = originalMaterial;
        targetScale = originalScale;
    }

    private void OnMouseDown()
    {
        targetScale = pressedScale;
    }

    private void OnMouseUp()
    {
        targetScale = (objectRenderer.material == hoverMaterial) ? originalScale * hoverScale : originalScale;
    }

    private void OnMouseUpAsButton()
    {
        // Re-check for manager just in case it was lost during scene transitions
        if (menuManager == null) FindManager();

        if (menuManager != null)
        {
            // Reset visuals immediately so it's ready for the next time the scene loads
            objectRenderer.material = originalMaterial;
            transform.localScale = originalScale;

            menuManager.PlayGame();
        }
        else
        {
            Debug.LogError("Play Button: No MainMenuManager found!");
        }
    }
}