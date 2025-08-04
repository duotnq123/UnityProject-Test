using UnityEngine;

public class Skill1 : SkillBase
{
    [Header("Spawn Points")]
    public Transform projectileSpawnPoint;
    public Transform auraSpawnPoint;

    [Header("Object Pools")]
    public ObjectPool projectilePool;
    public ObjectPool auraPool;
    public ObjectPool explosionPool; // <-- Pool hiệu ứng nổ

    private GameObject activeAura;

    protected override void Activate()
    {
        animator.SetTrigger("skill1");
        movement.isMovementLocked = true;
    }

    public void ShowAura()
    {
        if (auraPool != null && auraSpawnPoint != null)
        {
            activeAura = auraPool.GetObject(auraSpawnPoint.position, auraSpawnPoint.rotation);
            activeAura.transform.SetParent(transform);
        }
    }

    public void FireProjectile()
    {
        if (projectilePool == null || projectileSpawnPoint == null) return;

        GameObject projectile = projectilePool.GetObject(projectileSpawnPoint.position, Quaternion.identity);
        if (projectile == null) return;

        // Hướng bắn
        Vector3 direction = transform.forward;
        Transform target = autoAim?.GetTarget();
        if (target != null)
        {
            direction = (target.position - projectileSpawnPoint.position).normalized;
        }

        projectile.transform.rotation = Quaternion.LookRotation(direction);

        // Gán dữ liệu
        var pc = projectile.GetComponent<ProjectileController>();
        if (pc != null)
        {
            pc.SetDirection(direction);
            pc.SetPool(projectilePool);
            pc.SetExplosionPool(explosionPool); // Gán Pool hiệu ứng nổ tại đây
        }
    }

    public void OnSkillEnd()
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