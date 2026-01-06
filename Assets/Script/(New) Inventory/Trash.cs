using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Trash : MonoBehaviour
{
    public Gate linkedGate;

    [Header("Object Settings")]
    public bool isBig = true;
    public bool canFloat = false;

    private ItemPickup itemPickup;
    private Rigidbody rb;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private float shrinkTimer = 0f;

    [Header("Shrink Settings")]
    public float desiredScale = 0.2f;
    public float shrinkDuration = 0.5f;

    private bool isShrinking = false;
    private bool isInWater = false;

    [Header("Float Settings")]
    public float minFloatForce = 2f;    // minimum upward force
    public float maxFloatForce = 4f;    // maximum upward force
    public float floatDamping = 0.5f;   // how smoothly it floats (less = bouncy)
    private float originalGravityScale;
    public ParticleSystem glowEffect;
    void Start()
    {
        itemPickup = GetComponent<ItemPickup>();
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;

        rb.useGravity = true;
    }

    void Update()
    {
        // Handle shrinking
        if (isShrinking)
        {
            shrinkTimer += Time.deltaTime;
            float t = Mathf.Clamp01(shrinkTimer / shrinkDuration);

            transform.localScale = Vector3.Lerp(originalScale, targetScale, t);

            if (t >= 1f)
            {
                isShrinking = false;
                isBig = false;

                if (linkedGate != null)
                {
                    linkedGate.trashObjects.Remove(gameObject);
                    linkedGate.RemoveTrash(this.gameObject, 0.5f, 0.5f);
                    Instantiate(glowEffect, transform.position, Quaternion.identity);
                }
            }
        }

        // Floating behavior
        if (isInWater && canFloat && !isBig)
        {
            ApplyFloatForce();
        }
    }

    public void Shrink()
    {
        if (isBig && !isShrinking)
        {
            targetScale = originalScale * desiredScale;
            shrinkTimer = 0f;
            isShrinking = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            if (!isBig) return;
            isInWater = true;

            if (canFloat)
            {
                rb.useGravity = false;
                rb.linearDamping = floatDamping;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isInWater = false;

            // Restore normal physics
            rb.useGravity = true;
            rb.linearDamping = 0f;
        }
    }

    private void ApplyFloatForce()
    {
        // Apply upward float force randomly
        float floatForce = Random.Range(minFloatForce, maxFloatForce);
        rb.AddForce(Vector3.up * floatForce, ForceMode.Acceleration);
    }
}
