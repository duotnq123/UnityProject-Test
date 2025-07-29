using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class AOEDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;
    public float tickInterval = 1f;
    public float duration = 2f;
    public LayerMask enemyLayer;

    [Header("Knockback Settings")]
    public bool enableKnockback = false;
    public float knockbackForce = 5f;
    public Vector3 knockbackExtraDirection = Vector3.up;

    private List<Collider> enemiesInRange = new List<Collider>();
    private Coroutine damageRoutine;
    public bool autoDisable = true;

    void OnEnable()
    {
        enemiesInRange.Clear();
        damageRoutine = StartCoroutine(DamageOverTime());
    }

    void OnDisable()
    {
        if (damageRoutine != null)
            StopCoroutine(damageRoutine);

        enemiesInRange.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) == 0) return;
        if (!enemiesInRange.Contains(other))
            enemiesInRange.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (enemiesInRange.Contains(other))
            enemiesInRange.Remove(other);
    }

    IEnumerator DamageOverTime()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                Collider col = enemiesInRange[i];
                if (col == null) continue;

                EnemyHealth enemy = col.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    // Knockback luôn (ngay cả khi không gây damage)
                    if (enableKnockback)
                        ApplyKnockback(col);

                    // Gây damage nếu chưa chết
                    if (!enemy.isDead)
                        enemy.TakeDamage(damage);
                }
            }

            elapsed += tickInterval;
            yield return new WaitForSeconds(tickInterval);
        }

        DisableAOE();
    }

    void ApplyKnockback(Collider col)
    {
        EnemyHealth enemy = col.GetComponent<EnemyHealth>();
        if (enemy == null) return;

        Vector3 direction = (col.transform.position - transform.position);
        direction.y = 0f;
        direction.Normalize();

        Vector3 finalDir = direction + knockbackExtraDirection.normalized;
        finalDir.Normalize();

        enemy.ApplyKnockback(finalDir, knockbackForce);
    }


    IEnumerator ReenableNavMesh(NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (agent != null) agent.enabled = true;
    }

    void DisableAOE()
    {
        if (autoDisable)
        gameObject.SetActive(false); // hoặc Destroy nếu không dùng pooling
    }
}
