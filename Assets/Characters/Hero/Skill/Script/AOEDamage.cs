using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AOEDamage : MonoBehaviour
{
    public float damage = 20f;
    public float duration = 2f; // Thời gian tồn tại của vùng AOE
    public LayerMask enemyLayer;

    private bool canDamage = true;

    void OnEnable()
    {
        canDamage = true;
        Invoke(nameof(DisableAOE), duration);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!canDamage) return;

        // Chỉ xử lý nếu nằm trong enemyLayer (vẫn dùng chung cho breakable object nếu muốn)
        if ((enemyLayer.value & (1 << other.gameObject.layer)) == 0) return;

        // Gây sát thương cho enemy
        EnemyHealth enemy = other.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Phá object có thể phá vỡ (Breakable)
        BreakableObject breakable = other.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.Break(); // hoặc breakable.TakeDamage(...) nếu có HP
        }
    }

    void DisableAOE()
    {
        canDamage = false;

        // Nếu dùng object pool → set inactive
        gameObject.SetActive(false);

        // Nếu không dùng object pool → có thể dùng Destroy(this.gameObject);
        // Destroy(gameObject);
    }
}
