using UnityEngine;

public class MeleeAttack : IAttackStrategy
{
    public void ExecuteAttack(Transform attackPoint, float range, int damage, LayerMask playerLayer)
    {
        float radius = range * 0.8f;
        Collider[] hits = Physics.OverlapSphere(attackPoint.position, radius, playerLayer);

        foreach (var hit in hits)
        {
            PlayerHealth health = hit.GetComponent<PlayerHealth>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Debug.Log($"Melee attack hit {hit.name} for {damage} damage.");
            }
        }
    }
}
