using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    // Combat settings for the enemy (how far they see, damage, attack rate)
    [Header("Combat Settings")]
    public float lookRadius = 15f;         // Detection radius for player
    public float damage = 10f;             // Damage dealt to player per shot
    public float attackCooldown = 1.5f;    // Time between attacks

    [Header("Distance Settings")]
    public float stoppingDistance = 5f;    // Distance to stop moving and shoot

    // Health settings
    [Header("Health")]
    public int maxHealth = 100;            // Enemy max health
    private int currentHealth;             // Enemy current health

    // External references for UI, death effect, and shooting
    [Header("References")]
    public EnemyHealthBar healthBar;       // The enemy's healthbar UI
    public GameObject ragdollPrefab;       // Ragdoll spawned on death
    public float ragdollDespawnTime = 10f; // Time before ragdoll disappears
    public Transform muzzlePoint;          // Where bullets come from
    public LayerMask playerMask;           // Layer mask for hitting the player
    public Transform visualRoot;           // The part of the enemy that rotates (usually upper body)

    // Internal variables
    private Transform target;              // Reference to player
    private NavMeshAgent agent;            // NavMesh Agent for movement
    private Animator animator;             // Animator controller
    private float lastAttackTime;          // Timer to track attacks

    private void Start()
    {
        currentHealth = maxHealth;

        // Setup healthbar if assigned
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.lookAtTarget = Camera.main.transform;
        }

        // Find player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            target = player.transform;
        else
            Debug.LogError("No GameObject tagged 'Player' found.");

        // Setup agent and animator
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false; // Manual rotation control(Stops agent from snapping me in one direction)
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

        // If player is within detection radius
        if (distance <= lookRadius)
        {
            agent.SetDestination(target.position);

            // Animate running if moving
            bool isMoving = agent.velocity.magnitude > 0.1f;
            animator.SetBool("isRunning", isMoving);

            if (isMoving)
            {
                RotateToVelocity();
            }

            // Checks if the enemy is within shooting range
            if (distance <= stoppingDistance)
            {
                agent.isStopped = true;
                FaceTarget();

                // Handles shooting cooldown and animation
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

    // Rotates enemy visual root to match movement direction(FINALLY WORKS YES)
    void RotateToVelocity()
    {
        if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 moveDir = agent.velocity.normalized;
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, targetRotation, Time.deltaTime * 10f);

            // Debug green line to show rotation direction
            Debug.DrawRay(visualRoot.position + Vector3.up * 1f, moveDir * 3f, Color.green);
        }
    }

    // Rotates visual root directly at player(Fixes a weird turning bug)
    void FaceTarget()
    {
        Vector3 direction = (target.position - visualRoot.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
        visualRoot.rotation = Quaternion.Slerp(visualRoot.rotation, lookRotation, Time.deltaTime * 10f);
    }

    // Handles shooting logic using Raycast
    void ShootAtPlayer()
    {
        if (muzzlePoint == null) return;

        // Debug red line to show shot direction in scene view
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

    // Handle taking damage
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

    // Handle death, ragdoll spawn, and informing GameManager
    void Die()
    {
        // Tell GameManager this enemy died
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.EnemyKilled();
        }

        // Spawn ragdoll with force
        if (ragdollPrefab != null)
        {
            GameObject ragdoll = Instantiate(ragdollPrefab, transform.position + Vector3.up * 0.1f, transform.rotation);
            Rigidbody rb = ragdoll.GetComponent<Rigidbody>();
            if (rb != null)
                rb.AddForce(-transform.forward * 8f + Vector3.up * 2f, ForceMode.Impulse);

            Destroy(ragdoll, ragdollDespawnTime);
        }

        // Destroy enemy object
        Destroy(gameObject);
    }

    // Visualize detection radius in scene view
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
