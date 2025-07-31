using UnityEngine;

public class Skill4 : SkillBase
{
    [Header("Spawn Points")]
    public Transform auraSpawnPoint;

    [Header("Object Pools")]
    public ObjectPool projectilePool;  
    public ObjectPool auraPool;

    private GameObject activeAura;

    protected override void Activate()
    {
        animator.SetTrigger("skill4");
        movement.isMovementLocked = true;
    }

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
        if (projectilePool == null) return;

        Transform target = autoAim?.GetTarget();
        if (target == null) return;

        projectilePool.GetObject(target.position, Quaternion.identity);
        // AOE tự lo việc return sau khi hết hiệu ứng, không cần xử lý ở đây
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
