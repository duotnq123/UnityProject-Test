using UnityEngine;

public class Skill4 : SkillBase
{
    public Transform projectileSpawnPoint;
    public Transform auraSpawnPoint;

    public ObjectPool projectilePool; // Gán trong Inspector
    public ObjectPool auraPool;       // Gán trong Inspector

    private GameObject activeAura;

    protected override void Activate()
    {
        animator.SetTrigger("skill4");
        movement.isMovementLocked = true;
    }

    // Animation Event
    public void ShowAura_4()
    {
        if (activeAura == null && auraPool && auraSpawnPoint)
        {
            activeAura = auraPool.GetObject(auraSpawnPoint.position, auraSpawnPoint.rotation);
            activeAura.transform.SetParent(transform);
        }
    }

    public void FireProjectile_4()
    {
        if (projectilePool && projectileSpawnPoint)
        {
            GameObject projectile = projectilePool.GetObject(projectileSpawnPoint.position, Quaternion.identity);

            Transform target = autoAim?.GetTarget();

            Vector3 dir;
            if (target != null)
                dir = (target.position - projectileSpawnPoint.position).normalized;
            else
                dir = transform.forward;

            projectile.transform.rotation = Quaternion.LookRotation(dir);

            // Gán hướng di chuyển cho ProjectileController
            ProjectileController pc = projectile.GetComponent<ProjectileController>();
            if (pc != null)
            {
                pc.SetDirection(dir);
            }
        }
    }

    public void OnSkillEnd_4()
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
