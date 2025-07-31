using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    public float damage = 20f;
    public bool enableKnockback = false;
    public float knockbackForce = 5f;
    public Vector3 knockbackExtraDirection = Vector3.up;
    public LayerMask enemyLayer;

    private void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null && !enemy.isDead)
        {
            if (enableKnockback)
                ApplyKnockback(enemy, other.transform.position);

            enemy.TakeDamage(damage);
        }
        gameObject.SetActive(false); 
    }

    void ApplyKnockback(EnemyHealth enemy, Vector3 hitPoint)
    {
        Vector3 direction = (enemy.transform.position - hitPoint).normalized;
        Vector3 finalDir = direction + knockbackExtraDirection.normalized;
        finalDir.Normalize();

        enemy.ApplyKnockback(finalDir, knockbackForce);
    }
}
