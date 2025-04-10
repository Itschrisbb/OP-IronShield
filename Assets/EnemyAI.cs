using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Combat Settings")]
    public float lookRadius = 15f;
    public float damage = 10f;
    public float attackCooldown = 1.5f;

    [Header("Distance Settings")]
    public float stoppingDistance = 5f;

    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    [Header("References")]
    public EnemyHealthBar healthBar;
    public GameObject ragdollPrefab;
    public float ragdollDespawnTime = 10f;
    public Transform muzzlePoint;
    public LayerMask playerMask;
    public Transform visualRoot;

    private Transform target;
    private NavMeshAgent agent;
    private Animator animator;
    private float lastAttackTime;

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.lookAtTarget = Camera.main.transform;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        else
            Debug.LogError("No GameObject tagged 'Player' found.");

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.stoppingDistance = stoppingDistance;

        animator = GetComponentInChildren<Animator>();
        if (animator == null)
            Debug.LogError("Animator not found on " + gameObject.name);

        animator.applyRootMotion = false;
    }

    private void Update()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);

            if (isMoving)
            {
                RotateToVelocity();
            }

            if (distance <= stoppingDistance)
            {
                agent.isStopped = true;
                FaceTarget();

                if (Time.time >= lastAttackTime + attackCooldown && !animator.IsInTransition(0))
                {
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (!stateInfo.IsTag("Shooting"))
                    {
                        animator.SetTrigger("shoot");
                        lastAttackTime = Time.time;
                        ShootAtPlayer();
                    }
                }
            }
            else
            {
                agent.isStopped = false;
            }
        }
    }

    void RotateToVelocity()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 moveDir = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, targetRotation, Time.deltaTime * 10f);

            Debug.DrawRay(visualRoot.position + Vector3.up * 1f, moveDir * 3f, Color.green);
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.position - visualRoot.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, lookRotation, Time.deltaTime * 10f);
    }

    void ShootAtPlayer()
    {
        if (muzzlePoint == null) return;

        Debug.DrawRay(muzzlePoint.position, muzzlePoint.forward * 50f, Color.red, 1f);

        RaycastHit hit;
        if (Physics.Raycast(muzzlePoint.position, muzzlePoint.forward, out hit, lookRadius, playerMask))
        {
            PlayerHealth playerHealth = hit.collider.GetComponentInParent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage((int)damage);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // Notify the GameManager
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.EnemyKilled();
        }

        // Spawn ragdoll
        if (ragdollPrefab != null)
        {
            GameObject ragdoll = Instantiate(ragdollPrefab, transform.position + Vector3.up * 0.1f, transform.rotation);
            Rigidbody rb = ragdoll.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(-transform.forward * 8f + Vector3.up * 2f, ForceMode.Impulse);

            Destroy(ragdoll, ragdollDespawnTime);
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
