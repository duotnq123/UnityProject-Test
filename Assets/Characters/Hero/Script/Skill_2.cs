using UnityEngine;

public class Skill2 : SkillBase
{
    [Header("Spawn Points")]
    public Transform projectileSpawnPoint;
    public Transform auraSpawnPoint;

    [Header("Object Pools")]
    public ObjectPool projectilePool;
    public ObjectPool auraPool;
    public ObjectPool explosionPool;

    private GameObject activeAura;

    protected override void Activate()
    {
        animator.SetTrigger("skill2");
        movement.isMovementLocked = true;
    }

    // Gọi bởi animation event
    public void ShowAura_2()
    {
        if (auraPool != null && auraSpawnPoint != null)
        {
            activeAura = auraPool.GetObject(auraSpawnPoint.position, auraSpawnPoint.rotation);
            activeAura.transform.SetParent(transform);
        }
    }

    // Gọi bởi animation event
    public void FireProjectile_2()
    {
        if (projectilePool == null || projectileSpawnPoint == null)
        {
            Debug.LogWarning("ProjectilePool hoặc SpawnPoint chưa được gán.");
            return;
        }

        GameObject projectile = projectilePool.GetObject(projectileSpawnPoint.position, Quaternion.identity);

        // Xác định hướng bắn
        Vector3 direction;
        Transform target = autoAim?.GetTarget();
        if (target != null)
        {
            direction = (target.position - projectileSpawnPoint.position).normalized;
        }
        else
        {
            direction = transform.forward;
        }

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

    // Gọi bởi animation event
    public void OnSkillEnd_2()
    {
        if (activeAura != null && auraPool != null)
        {
            auraPool.ReturnObject(activeAura);
            activeAura = null;
        }

        movement.isMovementLocked = false;
        SkillBase.isSkillPlaying = false;
    }
}
