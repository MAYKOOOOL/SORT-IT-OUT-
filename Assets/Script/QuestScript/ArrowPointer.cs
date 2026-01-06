using UnityEngine;

public class ArrowPointer : MonoBehaviour
{
    public GameObject arrowGraphic;
    public Transform currentTarget;

    [Header("Settings")]
    public float hideDistance = 2.0f; // Distance at which the arrow hides

    public void SetTarget(Transform target)
    {
        currentTarget = target;
        // We let Update() decide whether to show it immediately based on distance
    }

    public void DisableArrow()
    {
        currentTarget = null;
        if (arrowGraphic.activeSelf) arrowGraphic.SetActive(false);
    }

    void Update()
    {
        if (currentTarget != null)
        {
            // 1. Calculate distance
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            // 2. Check if we are too close
            if (distance <= hideDistance)
            {
                // Hide arrow
                if (arrowGraphic.activeSelf) arrowGraphic.SetActive(false);
            }
            else
            {
                // Show arrow
                if (!arrowGraphic.activeSelf) arrowGraphic.SetActive(true);

                // 3. Perform Rotation Logic
                Vector3 direction = currentTarget.position - transform.position;
                direction.y = 0;

                if (direction == Vector3.zero) return;

                Quaternion targetLookRotation = Quaternion.LookRotation(direction);
                float targetYAngle = targetLookRotation.eulerAngles.y;

                transform.rotation = Quaternion.Euler(
                    transform.rotation.eulerAngles.x,
                    targetYAngle,
                    transform.rotation.eulerAngles.z
                );
            }
        }
    }
}