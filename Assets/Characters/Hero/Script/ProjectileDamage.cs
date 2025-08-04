using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class ProjectileDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;
    public bool enableKnockback = false;
    public float knockbackForce = 5f;
    public Vector3 knockbackExtraDirection = Vector3.up;
    public LayerMask enemyLayer;

    private ObjectPool explosionPool; // <-- Không kéo tay, set bằng code

    [HideInInspector] public Action<Vector3, Collider> onHitCallback;

    public void SetExplosionPool(ObjectPool pool)
    {
        explosionPool = pool;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        Vector3 hitPos = other.ClosestPoint(transform.position);
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy != null && !enemy.isDead)
        {
            if (enableKnockback)
                ApplyKnockback(enemy, hitPos);

            enemy.TakeDamage(damage);
        }

        // Gọi hiệu ứng nổ
        if (explosionPool != null)
        {
            explosionPool.GetObject(hitPos, Quaternion.identity);
        }

        onHitCallback?.Invoke(hitPos, other);

        gameObject.SetActive(false);
    }

    void ApplyKnockback(EnemyHealth enemy, Vector3 hitPoint)
    {
        Vector3 direction = (enemy.transform.position - hitPoint).normalized;
        Vector3 finalDir = direction + knockbackExtraDirection.normalized;
        enemy.ApplyKnockback(finalDir.normalized, knockbackForce);
    }
}
