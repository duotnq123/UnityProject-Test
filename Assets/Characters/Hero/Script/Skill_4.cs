using UnityEngine;
using System.Collections;

public class Skill4 : SkillBase
{
    [Header("Spawn Points")]
    public Transform auraSpawnPoint;

    [Header("Object Pools")]
    public ObjectPool projectilePool;
    public ObjectPool auraPool;
    public ObjectPool explosionPool;

    [Header("Explosion Settings")]
    public float explosionInterval = 0.5f; // Thời gian giữa các lần nổ
    public float explosionDuration = 3f;   // Tổng thời gian nổ liên tục

    private GameObject activeAura;
    private Coroutine explosionRoutine;

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

        // Gọi AOE tĩnh tại vị trí mục tiêu
        projectilePool.GetObject(target.position, Quaternion.identity);

        // Bắt đầu vòng lặp nổ liên tục
        if (explosionRoutine != null) StopCoroutine(explosionRoutine);
        explosionRoutine = StartCoroutine(ExplosionLoopRoutine(target.position));
    }

    private IEnumerator ExplosionLoopRoutine(Vector3 position)
    {
        float elapsed = 0f;

        while (elapsed < explosionDuration)
        {
            if (explosionPool != null)
            {
                explosionPool.GetObject(position, Quaternion.identity);
            }

            yield return new WaitForSeconds(explosionInterval);
            elapsed += explosionInterval;
        }
    }

    public void OnSkillEnd_4()
    {
        if (explosionRoutine != null)
        {
            StopCoroutine(explosionRoutine);
            explosionRoutine = null;
        }

        if (activeAura != null && auraPool != null)
        {
            auraPool.ReturnObject(activeAura);
            activeAura = null;
        }

        movement.isMovementLocked = false;
        SkillBase.isSkillPlaying = false;
    }
}
