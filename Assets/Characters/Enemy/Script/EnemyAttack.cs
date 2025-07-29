using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
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
    public bool isDead = false;
    private bool isTemporarilyDisabled = false;

    private Coroutine attackCoroutine;

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
        if (isDead || isTemporarilyDisabled || target == null || !agent.isOnNavMesh)
            return;

        if (isAttacking)
        {
            // Ngăn mọi di chuyển trong lúc attack
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            return;
        }

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            FaceTarget();

            if (attackCoroutine == null)
            {
                attackCoroutine = StartCoroutine(AttackRoutine());
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
        }
    }


    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        agent.isStopped = true;
        animator.SetTrigger("Attack");
        FaceTarget();

        // Chờ animation tấn công hoàn tất thông qua Animation Event OnAttackEnd
        yield return new WaitForSeconds(attackCooldown);

        // Nếu Animation Event không gọi kịp thì fallback tại đây
        EndAttackSafely();
    }

    void FaceTarget()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    // Gọi từ Animation Event ở khung giữa
    public void DealDamage()
    {
        if (isDead || isTemporarilyDisabled || target == null) return;

        Vector3 hitCenter = transform.position + transform.forward * (attackRange * 0.5f);
        float radius = attackRange * 0.5f;

        Collider[] hits = Physics.OverlapSphere(hitCenter, radius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Enemy dealt damage to Player.");
                // hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

    // Gọi từ Animation Event ở cuối clip tấn công
    public void OnAttackEnd()
    {
        EndAttackSafely();
        Debug.Log("Enemy finished attack animation.");
    }

    private void EndAttackSafely()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
        agent.isStopped = false;
    }

    public void SetTemporarilyDisabled(bool disabled)
    {
        isTemporarilyDisabled = disabled;

        if (disabled)
        {
            ResetAttack();
            agent.isStopped = true;
        }
    }

    public void ResetAttack()
    {
        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        isAttacking = false;
        animator.ResetTrigger("Attack");
        agent.isStopped = false;
    }

    public void SetDead(bool value)
    {
        isDead = value;
        if (value)
        {
            ResetAttack();
            agent.isStopped = true;
        }
    }
}
