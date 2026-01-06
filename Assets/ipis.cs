using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ipis : MonoBehaviour
{
    [Header("Ranges")]
    public float detectRange = 15f;
    public float attackRange = 5f;

    [Header("Dash Settings")]
    public float dashSpeed = 10f;
    public float dashDuration = 0.4f;

    [Header("Rotation Before Dash")]
    public float rotateBeforeDashTime = 0.25f; // how long to rotate before dash
    public float rotateSpeed = 10f;            // how fast the rotation is

    [Header("Cooldown")]
    public float dashCooldown = 3f;
    private float nextDashTime = 0f;

    [HideInInspector] public bool playerDetected = false;
    public Transform player;

    [Header("Roaming")]
    public float roamRadius = 20f;
    public float waitTime = 2f;

    [HideInInspector] public float speed;

    private NavMeshAgent agent;
    private Animator anim;
    private bool isWaiting = false;
    private bool isDashing = false;
    public float health = 10;
    public trig_col trig;
    private Vector3 lastDestination;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        trig = GetComponentInChildren<trig_col>();
        speed = agent.speed;
        SetNewDestination();
    }

    void Update()
    {
        if (isDashing) return;

        if (playerDetected && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // ? Chase when detected but far
            if (distance > attackRange)
            {
                agent.speed = speed * 2; // chase faster
                agent.SetDestination(player.position);
                UpdateAnimations();
            }

            // ? Dash when close AND cooldown ready
            if (distance <= attackRange && Time.time >= nextDashTime)
            {
                StartCoroutine(DashAttack());
                return;
            }
        }
        else
        {
            // ? Normal roaming
            agent.speed = speed;
            if (!isWaiting && agent.remainingDistance <= agent.stoppingDistance)
                StartCoroutine(WaitAndRoam());
        }
    }

    IEnumerator DashAttack()
    {
        isDashing = true;

        // Disable NavMesh so dash can ignore navmesh
        agent.enabled = false;

        anim.SetTrigger("Attack");

        // -------- SMOOTH ROTATION TOWARD PLAYER --------
        float rotateTime = 0f;
        while (rotateTime < rotateBeforeDashTime)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;

            Quaternion targetRot = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            rotateTime += Time.deltaTime;
            yield return null;
        }
        // -------------------------------------------------

        // Dash direction AFTER rotation finished
        Vector3 dashDirection = (player.position - transform.position).normalized;

        // -------- DASH MOVEMENT --------
        float t = 0f;
        while (t < dashDuration)
        {
            transform.position += dashDirection * dashSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
        // --------------------------------

        // Re-enable NavMesh
        agent.enabled = true;

        // Snap back to navmesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 3f, NavMesh.AllAreas))
            transform.position = hit.position;

        // Set cooldown
        nextDashTime = Time.time + dashCooldown;

        isDashing = false;
    }

    IEnumerator WaitAndRoam()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        SetNewDestination();
        isWaiting = false;
    }

    public void SetNewDestination()
    {
        Vector3 randomDirection;
        NavMeshHit hit;
        int attempts = 0;
        float minDistance = 5f; // minimum distance from last destination

        do
        {
            randomDirection = Random.insideUnitSphere * roamRadius + transform.position;
            attempts++;
        } while (Vector3.Distance(randomDirection, lastDestination) < minDistance && attempts < 20);

        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, NavMesh.AllAreas))
        {
            lastDestination = hit.position; // save this as last
            agent.SetDestination(hit.position);
        }
    }

    void UpdateAnimations()
    {
        bool isMoving = agent.enabled && agent.velocity.magnitude > 0.1f;
        anim.SetBool("Run", isMoving);
        anim.SetBool("Idle", !isMoving);
    }
    public void take(int amount)
    {
        health -= amount;
        if (health <= 0)
        {
            // Check 1: Ensure the QuestSystem exists
            if (QuestSystem.Instance != null && QuestSystem.Instance.activeQuest != null)
            {
                // Check 2: Check if the active quest is the right type
                if (QuestSystem.Instance.activeQuest.questType == QuestType.DestroyEnemy)
                {
                    // Report the kill. We pass -1 as a placeholder ID.
                    QuestSystem.Instance.ReportEnemyDestroyed(-1);
                }
            }

            // Destroy the GameObject.
            Destroy(gameObject);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            trig.stats.TakeDamage(5);
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            take(5);
        }
    }
}
