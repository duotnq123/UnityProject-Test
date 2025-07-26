using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isDead { get; private set; } = false;

    private Animator animator;
    private Collider col;
    private NavMeshAgent agent;
    private EnemyPatrol enemyPatrol;
    private bool isTakingDamage = false;


    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
        enemyPatrol = GetComponent<EnemyPatrol>();
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

            if (animator != null)
                animator.SetTrigger("TakeDamage");

            var attack = GetComponent<EnemyAttack>();
            if (attack != null) attack.ResetAttack();

            if (enemyPatrol != null && enemyPatrol.isInAlarmState)
            {
                enemyPatrol.CancelAlarm();
            }
        }
    }

    public void OnTakeDamageEnd()
    {
        if (isDead) return;

        isTakingDamage = false;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = false;

        var attack = GetComponent<EnemyAttack>();
        if (attack != null)
        {
            attack.OnHitEnd();
        }
    }
    void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Die");

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        if (col != null)
            col.enabled = false;

        if (enemyPatrol != null)
            enemyPatrol.isDead = true;

        var attack = GetComponent<EnemyAttack>();
        if (attack != null)
            attack.isDead = true;

        // Báo cho PlayerAutoAim nếu đang lock enemy này
        var playerAim = FindAnyObjectByType<PlayerAutoAim>();
        if (playerAim != null)
            playerAim.OnEnemyDied(transform);
    }

    public void OnDeathAnimationEnd()
    {
        gameObject.SetActive(false);
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;

        if (col != null)
            col.enabled = true;

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = false;

        animator.Rebind(); // Reset lại Animator
    }
}
