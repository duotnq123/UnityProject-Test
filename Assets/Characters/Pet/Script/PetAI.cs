using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class PetAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;

    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float damage = 20f;
    private float lastAttackTime;
    private bool isAttacking = false;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private Vector3 previousPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void OnEnable()
    {
        agent.speed = moveSpeed;
        agent.updateRotation = false;
        isAttacking = false;
        lastAttackTime = 0f;
        FindNewTarget();
    }

    void Update()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            FindNewTarget();
            return;
        }

        EnemyHealth enemyHealth = currentTarget.GetComponent<EnemyHealth>();
        if (enemyHealth == null || enemyHealth.isDead)
        {
            FindNewTarget();
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            RotateTowards(currentTarget.position);

            if (!isAttacking && Time.time - lastAttackTime >= attackCooldown)
            {
                animator.SetTrigger("Attack");
                isAttacking = true;
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
            RotateTowards(agent.steeringTarget);
            isAttacking = false;
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        float actualSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed, 0.1f, Time.deltaTime);
        previousPosition = transform.position;
    }

    void RotateTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (GameObject enemy in enemies)
        {
            if (enemy == null) continue;

            EnemyHealth health = enemy.GetComponent<EnemyHealth>();
            if (health == null || health.isDead) continue;

            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy.transform;
            }
        }

        currentTarget = closest;
    }

    // Gọi từ Animation Event
    public void DealDamage()
    {
        if (currentTarget == null) return;

        EnemyHealth enemy = currentTarget.GetComponent<EnemyHealth>();
        if (enemy == null || enemy.isDead) return;

        enemy.TakeDamage(damage);
    }

    // Gọi từ cuối animation Attack (animation event)
    public void OnAttackEnd()
    {
        isAttacking = false;
    }
}
