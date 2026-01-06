using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public MouseItem mouseItem = new MouseItem();
    public Transform model;
    [Header("Permanent Upgrades")]
    public float permanentSpeedBoost = 0f;
    public float permanentJumpBoost = 0f;
    public float permanentThrowBoost = 0f;

    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float jumpForce = 5.0f;
    public float gravity = 9.8f;
    public float rotationSpeed = 10.0f;

    [Header("Ripple Effect")]
    public ParticleSystem ripple;
    private bool inWater;

    private float VelocityXZ;
    private float VelocityY;
    private Vector3 PlayerPos;

    private CharacterController controller;
    private PlayerStats stats;
    private Vector3 velocity;
    private bool isGrounded;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        stats = GetComponent<PlayerStats>();
        PlayerPos = transform.position;
    }

    private void Update()
    {
        if (!PlayerController.Instance.onSwim)
            isGrounded = controller.isGrounded;

        Vector2 input = PlayerController.Instance.move;
        Vector3 move = new Vector3(input.x, 0, input.y);

        // Smooth rotate
        if (PlayerController.Instance.onAim)
        {
            RotateTowardsMouseScreen();
        }
        // When not aiming: rotate toward movement direction
        else if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move, Vector3.up);
            model.transform.rotation = Quaternion.Slerp(model.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        float speed = walkSpeed + permanentSpeedBoost;

        if (PlayerController.Instance.onRun && stats.HasEnoughEnergy(0.1f))
        {
            speed = runSpeed + permanentSpeedBoost;
            stats.DrainEnergy(stats.energyDrainRate * Time.deltaTime);
        }
        else if (PlayerController.Instance.onCrouch)
        {
            speed = crouchSpeed;
        }

        // Apply horizontal movement
        controller.Move(move.normalized * speed * Time.deltaTime);

        // Gravity & Jumping
        if (isGrounded && !PlayerController.Instance.onSwim && velocity.y < 0)
            velocity.y = -2f;

        //Langoy
        if (PlayerController.Instance.onSwim)
        {
            gravity = 5;
            speed = crouchSpeed;

            if (PlayerController.Instance.onJump)
                velocity.y = jumpForce + permanentJumpBoost;
            else if (PlayerController.Instance.onSwimDown)
                velocity.y = -jumpForce * 0.5f;
            else
                velocity.y = 0f;
        }
        else
        {
            gravity = 9.8f;

            if (!isGrounded)
                velocity.y -= gravity * Time.deltaTime;

            if (PlayerController.Instance.onJump && isGrounded)
                velocity.y = jumpForce + permanentJumpBoost;
        }

        // Apply vertical movement
        controller.Move(velocity * Time.deltaTime);

        // Velocity tracking for ripple movement
        VelocityXZ = Vector3.Distance(
            new Vector3(transform.position.x, 0, transform.position.z),
            new Vector3(PlayerPos.x, 0, PlayerPos.z)
        );

        VelocityY = Mathf.Abs(transform.position.y - PlayerPos.y);
        PlayerPos = transform.position;
    }

    // --- FOOTSTEPS INTEGRATION HELPERS ---

    /// <summary>
    /// Checks if the player is currently moving on solid ground (not swimming or airborne).
    /// </summary>
    public bool IsWalkingOnGround()
    {
        return PlayerController.Instance.move.magnitude > 0.1f && isGrounded && !PlayerController.Instance.onSwim;
    }

    /// <summary>
    /// Checks if the player is currently moving inside a water volume.
    /// </summary>
    public bool IsWalkingInWater()
    {
        // Checks if the player is flagged as 'inWater' (via trigger) AND has horizontal movement
        return inWater && PlayerController.Instance.move.magnitude > 0.1f;
    }

    /// <summary>
    /// Checks if the player is holding the run input.
    /// </summary>
    public bool IsRunning()
    {
        return PlayerController.Instance.onRun;
    }

    // --- RIPPLES (Your existing ripple logic) ---

    void CreateRipple(int Start, int End, int Delta, float Speed, float Size, float Lifetime)
    {
        Vector3 forward = ripple.transform.eulerAngles;
        forward.y = Start;
        ripple.transform.eulerAngles = forward;

        for (int i = Start; i < End; i += Delta)
        {
            ripple.Emit(
                transform.position + ripple.transform.forward * 1.15f,
                ripple.transform.forward * Speed,
                Size,
                Lifetime,
                Color.white
            );

            ripple.transform.Rotate(Vector3.up * Delta, Space.World);
        }
    }

    private void RotateTowardsMouseScreen()
    {
        Vector3 mousePos = Input.mousePosition;

        // Screen center (where your character always is)
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);

        // Direction from center → mouse
        Vector3 dir = mousePos - screenCenter;

        // Convert to world direction (XZ)
        Vector3 worldDir = new Vector3(dir.x, 0f, dir.y).normalized;

        if (worldDir.sqrMagnitude > 0.001f)
        {
            Quaternion target = Quaternion.LookRotation(worldDir);
            model.rotation = Quaternion.Slerp(model.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 4)  // Water
        {
            inWater = true;

            // Splash on entering water
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 4 && inWater)
        {
            // walking ripples
            if (VelocityXZ > 0.025f && Time.renderedFrameCount % 3 == 0)
            {
                int y = (int)transform.eulerAngles.y;
                CreateRipple(y - 100, y + 100, 3, 5f, 2.65f, 3f);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 4)
        {
            inWater = false;

            // Splash on exit
            ripple.Emit(transform.position, Vector3.zero, 5, 0.1f, Color.white);
        }
    }
}