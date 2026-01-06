using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class rat : MonoBehaviour
{

    [Header("Ranges")]
    public float detectRange = 12f;
    public float attackRange = 2f;

    [Header("Attack Settings")]
    public float rotateBeforeAttackTime = 0.25f;  // time spent rotating before attack
    public float rotateSpeed = 10f;               // how fast it rotates toward player
    public float attackCooldown = 2f;             // cooldown between attacks
    public float nextAttackTime = 0f;

    [HideInInspector] public bool playerDetected = false;
    public Transform player;
    public float health = 50;
    [Header("Roaming")]
    public float roamRadius = 15f;
    public float waitTime = 2f;

    [HideInInspector] public float speed;

    private NavMeshAgent agent;
    private Animator anim;

    private bool isWaiting = false;
    public bool isAttacking = false;
    public rat_trigger trig;
    private Vector3 lastDestination;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        trig = GetComponentInChildren<rat_trigger>();
        speed = agent.speed;
        SetNewRoamDestination();
    }

    void Update()
    {
        float distance = 0;

        if (playerDetected && player != null)
        {
            distance = Vector3.Distance(transform.position, player.position);

            // Chase if detected but not in attack range
            if (distance > attackRange)
            {
                agent.speed = speed * 1.5f;
                agent.SetDestination(player.position);
                UpdateAnimations();
            }

            // Attack if in range AND cooldown ready
            if (distance <= attackRange && Time.time >= nextAttackTime && !isAttacking)
            {
                StartCoroutine(Attack());
            }
        }
        else
        {
            // Roaming
            agent.speed = speed;

            if (!isWaiting && agent.remainingDistance <= agent.stoppingDistance)
                StartCoroutine(RoamWait());
        }
        if(health <= 0)
        {
            Destroy(gameObject);
        }

        UpdateAnimations();
    }

    public void take(int amount)
    {
        health -= amount;
    }
    // ------------------ ATTACK ------------------
    public IEnumerator Attack()
    {
        isAttacking = true;
        agent.ResetPath(); // stop moving

        // -------- SMOOTH ROTATION BEFORE ATTACK --------
        float rotateTime = 0f;
        while (rotateTime < rotateBeforeAttackTime)
        {
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            directionToPlayer.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);

            rotateTime += Time.deltaTime;
            yield return null;
        }
        // -------------------------------------------------
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);
        anim.SetBool("Att", true);
        trig.stats.TakeDamage(15);
        // Attack animation length (adjust as needed)
        yield return new WaitForSeconds(1f);

        nextAttackTime = Time.time + attackCooldown;
        anim.SetBool("Att", false);

    }

    // ------------------ ROAMING ------------------
  public IEnumerator RoamWait()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        SetNewRoamDestination();
        isWaiting = false;
    }

    public void SetNewRoamDestination()
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


    // ------------------ ANIMATIONS ------------------
    void UpdateAnimations()
    {
        if (isAttacking)
        {
            // Enemy is attacking ? Run and Idle are both false
            anim.SetBool("Run", false);
            anim.SetBool("Idle", false);
            return;
        }

        bool isMoving = agent.enabled && agent.velocity.magnitude > 0.1f;
        anim.SetBool("Run", isMoving);
        anim.SetBool("Idle", !isMoving);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            take(10);
        }
    }
}
