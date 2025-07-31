using UnityEngine;

public class Skill2 : SkillBase
{
    public Transform projectileSpawnPoint;
    public Transform auraSpawnPoint;

    public ObjectPool projectilePool; // Kéo vào từ scene
    public ObjectPool auraPool;       // Kéo vào từ scene

    private GameObject activeAura;

    protected override void Activate()
    {
        animator.SetTrigger("skill2");
        movement.isMovementLocked = true;
    }

    // Animation Event
    public void ShowAura_2()
    {
        if (auraPool != null && auraSpawnPoint != null)
        {
            activeAura = auraPool.GetObject(auraSpawnPoint.position, auraSpawnPoint.rotation);
            activeAura.transform.SetParent(transform); // Gắn vào player
        }
    }

    public void FireProjectile_2()
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
                pc.SetPool(projectilePool); 
            }
        }
    }

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
