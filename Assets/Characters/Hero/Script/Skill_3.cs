using UnityEngine;

public class Skill3 : SkillBase
{
    [Header("Spawn Points")]
    public Transform projectileSpawnPoint;
    public Transform auraSpawnPoint;

    [Header("Object Pools")]
    public ObjectPool projectilePool; // kéo vào scene
    public ObjectPool auraPool;       // kéo vào scene
    public ObjectPool explosionPool;  // THÊM DÒNG NÀY

    private GameObject activeAura;

    protected override void Activate()
    {
        isInterruptible = false; // <-- KHÔNG cho phép bị ngắt khi dùng skill này
        animator.SetTrigger("skill3");
        movement.isMovementLocked = true;
    }


    // Animation Event
    public void ShowAura_3()
    {
        if (auraPool != null && auraSpawnPoint != null)
        {
            activeAura = auraPool.GetObject(auraSpawnPoint.position, auraSpawnPoint.rotation);
            activeAura.transform.SetParent(transform); // Gắn vào player
        }
    }

    // Animation Event
    public void FireProjectile_3()
    {
        if (projectilePool == null || projectileSpawnPoint == null)
        {
            Debug.LogWarning("ProjectilePool hoặc SpawnPoint chưa được gán.");
            return;
        }

        GameObject projectile = projectilePool.GetObject(projectileSpawnPoint.position, Quaternion.identity);

        // Xác định hướng
        Vector3 direction;
        Transform target = autoAim?.GetTarget();
        if (target != null)
            direction = (target.position - projectileSpawnPoint.position).normalized;
        else
            direction = transform.forward;

        projectile.transform.rotation = Quaternion.LookRotation(direction);

        // Cấu hình projectile
        ProjectileController pc = projectile.GetComponent<ProjectileController>();
        if (pc != null)
        {
            pc.SetDirection(direction);
            pc.SetPool(projectilePool);
            pc.SetExplosionPool(explosionPool); 
        }
        else
        {
            Debug.LogError("Projectile không có ProjectileController.");
        }
    }

    // Animation Event
    public void OnSkillEnd_3()
    {
        if (activeAura != null && auraPool != null)
        {
            auraPool.ReturnObject(activeAura);
            activeAura = null;
        }

        EndSkill(); // ← Gọi hàm cha để unlock và reset
    }
}
