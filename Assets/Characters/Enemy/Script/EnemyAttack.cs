using UnityEngine;
using UnityEngine.AI;

public class EnemyAttack : MonoBehaviour
{
    public float attackRange = 1.8f;
    public float attackCooldown = 1.5f;
    public LayerMask playerLayer;
    public int damage = 10;

    private Animator animator;
    private Transform target;
    private NavMeshAgent agent;
    private float lastAttackTime = -Mathf.Infinity;

    [HideInInspector] public bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform t)
    {
        target = t;
    }

    void Update()
    {
        if (isDead || target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
        }
    }

    // Gọi từ Animation Event
    public void DealDamage()
    {
        if (isDead) return;

        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward, attackRange, playerLayer);
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Enemy dealt damage to player!");
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }
}
