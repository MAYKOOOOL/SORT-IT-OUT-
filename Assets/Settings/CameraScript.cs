using UnityEngine;
[ExecuteAlways]
public class CameraScript : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;

    public WallChecker sensorNorth;
    public WallChecker sensorSouth;
    public WallChecker sensorEast;
    public WallChecker sensorWest;

    private bool lockX = false;
    private bool lockZ = false;

    void Update()
    {
        Vector3 desiredPosition = player.position + offset;

        // Clamp movement in Z direction
        if (sensorNorth != null && sensorNorth.isTouchingWall)
        {
            // Block forward (+Z) movement
            if (desiredPosition.z > transform.position.z)
            {
                desiredPosition.z = transform.position.z;
            }
        }

        if (sensorSouth != null && sensorSouth.isTouchingWall)
        {
            // Block backward (-Z) movement
            if (desiredPosition.z < transform.position.z)
            {
                desiredPosition.z = transform.position.z;
            }
        }

        // Clamp movement in X direction
        if (sensorEast != null && sensorEast.isTouchingWall)
        {
            // Block right (+X) movement
            if (desiredPosition.x > transform.position.x)
            {
                desiredPosition.x = transform.position.x;
            }
        }

        if (sensorWest != null && sensorWest.isTouchingWall)
        {
            // Block left (-X) movement
            if (desiredPosition.x < transform.position.x)
            {
                desiredPosition.x = transform.position.x;
            }
        }

        // Smooth follow
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

}
