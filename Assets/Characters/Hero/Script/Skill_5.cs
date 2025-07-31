using UnityEngine;
using System.Collections;

public class Skill5 : SkillBase
{
    [Header("Summon Settings")]
    public Transform summonPoint;
    public float summonDuration = 10f;

    [Header("Object Pool")]
    public ObjectPool petPool; // Dùng object pool thay vì prefab
    public ObjectPool auraPool;

    [Header("Aura Settings")]
    public Transform auraSpawnPoint;

    private GameObject currentPet;
    private GameObject activeAura;

    protected override void Activate()
    {
        if (petPool == null || summonPoint == null)
        {
            Debug.LogWarning("Thiếu pool hoặc điểm summon!");
            return;
        }

        if (currentPet != null)
        {
            Debug.Log("Pet vẫn còn tồn tại!");
            return;
        }

        currentPet = petPool.GetObject(summonPoint.position, summonPoint.rotation);

        animator?.SetTrigger("skill5");

        StartCoroutine(SummonDurationRoutine());
    }

    private IEnumerator SummonDurationRoutine()
    {
        yield return new WaitForSeconds(summonDuration);

        if (currentPet != null)
        {
            petPool.ReturnObject(currentPet);
            currentPet = null;
        }

        if (activeAura != null && auraPool != null)
        {
            auraPool.ReturnObject(activeAura);
            activeAura = null;
        }

        SkillBase.isSkillPlaying = false;
    }

    // Animation Event
    public void ShowAura_5()
    {
        if (auraPool != null && auraSpawnPoint != null)
        {
            activeAura = auraPool.GetObject(auraSpawnPoint.position, auraSpawnPoint.rotation);
            activeAura.transform.SetParent(transform);
        }
    }

    public void OnSkillEnd()
    {
        if (activeAura != null && auraPool != null)
        {
            auraPool.ReturnObject(activeAura);
            activeAura = null;
        }
    }
}
