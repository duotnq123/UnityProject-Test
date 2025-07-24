using UnityEngine;
using System.Collections;

public class Skill4 : SkillBase
{
    [Header("Spawn Points")]
    public Transform auraSpawnPoint;

    [Header("Object Pools")]
    public ObjectPool projectilePool; // Dùng để spawn AOE
    public ObjectPool auraPool;

    [Header("Settings")]
    public float aoeDuration = 2f; // Thời gian AOE tồn tại, có thể chỉnh trong Inspector

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

        GameObject aoe = projectilePool.GetObject(target.position, Quaternion.identity);

        // Tùy chọn: nếu bạn có script gây damage, có thể gọi tại đây

        StartCoroutine(DisableEffectAfterDelay(aoe, aoeDuration));
    }

    private IEnumerator DisableEffectAfterDelay(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (projectilePool != null)
            projectilePool.ReturnObject(effect);
        else
            Destroy(effect);
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
