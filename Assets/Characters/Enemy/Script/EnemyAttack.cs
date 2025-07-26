using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float attackRange = 1.8f;
    public float attackCooldown = 1.2f;
    public LayerMask playerLayer;
    public int damage = 10;

    private Animator animator;
    private NavMeshAgent agent;
    private Transform target;

    private bool isAttacking = false;
    private bool useFirstAttack = true;
    private float lastAttackTime = -999f;

    [HideInInspector] public bool isDead = false;

    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Update()
    {
        if (isDead || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (isAttacking)
        {
            // Đang attack thì không làm gì khác (chờ animation kết thúc)
            FaceTarget();
            return;
        }

        if (distanceToTarget <= attackRange)
        {
            agent.isStopped = true;
            FaceTarget();

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    private void FaceTarget()
    {
        Vector3 lookDir = new Vector3(target.position.x, transform.position.y, target.position.z);
        transform.LookAt(lookDir);
    }

    private void StartAttack()
    {
        if (target == null) return;
        float distance = Vector3.Distance(transform.position, target.position);
        if (distance > attackRange) return;

        isAttacking = true;
        lastAttackTime = Time.time;

        animator.SetBool("IsCombo", !useFirstAttack);
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        useFirstAttack = !useFirstAttack;

        agent.isStopped = true;
    }

    // GỌI từ cuối animation Attack qua animation event
    public void OnAttackEnd()
    {
        isAttacking = false;

        // Nếu target vẫn còn trong range, không cần SetDestination vì sẽ tự update ở Update()
        if (target != null)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            if (dist > attackRange)
            {
                agent.isStopped = false;
                agent.SetDestination(target.position);
            }
        }
    }

    public void ResetAttack()
    {
        isAttacking = false;
        useFirstAttack = true;
        animator.ResetTrigger("Attack");
        animator.SetBool("IsCombo", false);
    }

    public void OnHitEnd()
    {
        if (isDead || target == null) return;

        float dist = Vector3.Distance(transform.position, target.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;

            if (!isAttacking && Time.time >= lastAttackTime + attackCooldown)
            {
                StartAttack();
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }

    // Gọi từ animation event tại khung chạm
    public void DealDamage()
    {
        if (isDead) return;

        Vector3 hitPos = transform.position + transform.forward * (attackRange * 0.5f);
        Collider[] hits = Physics.OverlapSphere(hitPos, attackRange * 0.5f, playerLayer);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Enemy dealt damage to player!");  
            }
        }
    }
}
