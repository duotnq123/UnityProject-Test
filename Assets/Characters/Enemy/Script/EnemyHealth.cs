using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Attack Settings")]
    public Transform attackPoint; // ← Gắn transform ở tay
    public float attackRadius = 0.8f;
    public float attackDamage = 10f;
    public LayerMask playerLayer;

    [Header("State Flags")]
    public bool isDead { get; private set; } = false;
    private bool isTakingDamage = false;
    private bool isKnockedBack = false;

    [Header("Components")]
    private Animator animator;
    private Collider col;
    private NavMeshAgent agent;
    private EnemyPatrol patrol;
    private EnemyAttack attack;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        patrol = GetComponent<EnemyPatrol>();
        attack = GetComponent<EnemyAttack>();
    }

    public void TakeDamage(float amount)
    {
        if (isDead || !gameObject.activeInHierarchy) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Remaining: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isTakingDamage = true;

            if (agent != null && agent.isOnNavMesh)
                agent.isStopped = true;

            animator?.SetTrigger("TakeDamage");

            if (attack != null)
            {
                attack.ResetAttack();
                attack.SetTemporarilyDisabled(true);
            }

            if (patrol != null)
            {
                if (patrol.isInAlarmState)
                    patrol.CancelAlarm();

                patrol.SetTemporarilyDisabled(true);
                patrol.enabled = false;
            }
        }
    }

    public void OnTakeDamageEnd()
    {
        if (isDead) return;

        isTakingDamage = false;

        if (!isKnockedBack && agent != null && agent.isOnNavMesh)
            agent.isStopped = false;

        animator.ResetTrigger("TakeDamage");

        if (attack != null)
            attack.SetTemporarilyDisabled(false);

        if (patrol != null)
        {
            patrol.SetTemporarilyDisabled(false);
            patrol.enabled = true;
        }
    }

    public void ApplyKnockback(Vector3 direction, float force)
    {
        if (isDead || !gameObject.activeInHierarchy) return;

        isKnockedBack = true;

        if (agent != null && agent.enabled)
            agent.isStopped = true;

        if (attack != null)
        {
            attack.SetTarget(null);
            attack.SetTemporarilyDisabled(true);
        }

        if (patrol != null)
        {
            patrol.SetTemporarilyDisabled(true);
            patrol.enabled = false;
        }

        StartCoroutine(KnockbackRoutine(direction.normalized, force));
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        float duration = 0.25f;
        float timer = 0f;

        while (timer < duration)
        {
            transform.position += direction * force * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        if (isDead) yield break;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(transform.position);
            agent.isStopped = false;
        }

        if (patrol != null)
        {
            patrol.SetTemporarilyDisabled(false);
            patrol.enabled = true;
        }

        if (attack != null)
            attack.SetTemporarilyDisabled(false);

        isKnockedBack = false;
    }

    private void Die()
    {
        isDead = true;
        animator?.SetTrigger("Die");

        if (agent != null && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }

        if (col != null)
            col.enabled = false;

        if (patrol != null)
            patrol.isDead = true;

        if (attack != null)
            attack.isDead = true;

        var aim = FindAnyObjectByType<PlayerAutoAim>();
        aim?.OnEnemyDied(transform);
    }

    public void OnDeathAnimationEnd()
    {
        if (agent != null)
            agent.enabled = false;

        gameObject.SetActive(false);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        isTakingDamage = false;
        isKnockedBack = false;

        if (col != null)
            col.enabled = true;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = false;

        animator?.Rebind();
    }

    public void DealDamage()
    {
        if (isDead || attackPoint == null) return;

        Collider[] hitPlayers = Physics.OverlapSphere(attackPoint.position, attackRadius, playerLayer);
        foreach (var player in hitPlayers)
        {
            PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                Debug.Log($"Enemy hit {player.name} with {attackDamage} damage.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
        }
    }
}
