using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class AOEDamage : MonoBehaviour
{
    public float damage = 20f;
    public float tickInterval = 1f;    // Bao lâu gây damage 1 lần
    public float duration = 2f;        // Tổng thời gian tồn tại
    public LayerMask enemyLayer;

    private List<Collider> enemiesInRange = new List<Collider>();
    private Coroutine damageRoutine;

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
        while (true)
        {
            for (int i = enemiesInRange.Count - 1; i >= 0; i--)
            {
                Collider col = enemiesInRange[i];
                if (col == null) continue;

                // Gây damage cho Enemy
                EnemyHealth enemy = col.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }

                // Phá object có thể phá vỡ (Breakable)
                BreakableObject breakable = col.GetComponent<BreakableObject>();
                if (breakable != null)
                {
                    breakable.Break(); // hoặc .TakeDamage(...) nếu có HP
                }
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }

    void DisableAOE()
    {
        gameObject.SetActive(false); // hoặc Destroy(gameObject) nếu không dùng pool
    }
}
