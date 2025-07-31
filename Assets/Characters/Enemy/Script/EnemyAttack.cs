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

    [Header("Attack Point")]
    public Transform attackPoint;  // Gắn vào tay hoặc vũ khí

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
                attackCoroutine = StartCoroutine(AttackRoutine());
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

        yield return new WaitForSeconds(attackCooldown);

        EndAttackSafely(); // Fallback nếu animation không gọi OnAttackEnd
    }

    void FaceTarget()
    {
        if (target == null) return;

        Vector3 dir = target.position - transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.01f)
        {
            Quaternion lookRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    public void DealDamage()
    {
        if (isDead || isTemporarilyDisabled || target == null || attackPoint == null) return;

        float radius = attackRange * 0.5f;
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, radius);

        foreach (var hit in hits)
        {
            CharacterController controller = hit.GetComponent<CharacterController>();
            if (controller != null)
            {
                Debug.Log("Enemy dealt damage to Player via CharacterController.");
                hit.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            }
        }
    }

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

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, 0.8f);
        }
    }
#endif
}
