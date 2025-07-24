using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    private Animator animator;
    private Collider col;
    private NavMeshAgent agent;

    void Awake()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        col = GetComponent<Collider>();
        agent = GetComponent<NavMeshAgent>();
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
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");

        if (agent != null && agent.isOnNavMesh)
            agent.isStopped = true;

        if (col != null)
            col.enabled = false;

        var patrol = GetComponent<EnemyPatrol>();
        if (patrol != null) patrol.isDead = true;

        var attack = GetComponent<EnemyAttack>();
        if (attack != null) attack.isDead = true;
    }

    // Gọi từ Animation Event cuối animation chết
    public void OnDeathAnimationEnd()
    {
        gameObject.SetActive(false);
    }
}
