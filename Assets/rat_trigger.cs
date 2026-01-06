using UnityEngine;

public class rat_trigger : MonoBehaviour
{
    [Header("Player")]
    public PlayerStats stats;
    public rat ratEnemy; // Reference to the rat script

    void Start()
    {
        ratEnemy = GetComponentInParent<rat>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            stats = other.gameObject.GetComponent<PlayerStats>();
            ratEnemy.player = other.transform;  // Assign player
            ratEnemy.playerDetected = true;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ratEnemy.player = other.transform;
            ratEnemy.playerDetected = true;

            // Face the player
            Vector3 directionToPlayer = (ratEnemy.player.position - ratEnemy.transform.position).normalized;
            directionToPlayer.y = 0; // keep only horizontal rotation
            if (directionToPlayer != Vector3.zero)
            {
                ratEnemy.transform.rotation = Quaternion.Slerp(
                    ratEnemy.transform.rotation,
                    Quaternion.LookRotation(directionToPlayer),
                    ratEnemy.rotateSpeed * Time.deltaTime
                );
            }

            // Trigger attack if in range & cooldown ready
            float distance = Vector3.Distance(ratEnemy.transform.position, ratEnemy.player.position);
            if (distance <= ratEnemy.attackRange && Time.time >= ratEnemy.nextAttackTime && !ratEnemy.isAttacking)
            {
                ratEnemy.StartCoroutine(ratEnemy.Attack());
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ratEnemy.playerDetected = false;
            ratEnemy.isAttacking = false;
            ratEnemy.player = null;
        }
    }
}
