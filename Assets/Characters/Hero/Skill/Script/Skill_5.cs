using UnityEngine;
using System.Collections;

public class Skill5 : SkillBase
{
    [Header("Summon Settings")]
    public Transform summonPoint;
    public float summonDuration = 10f;

    [Header("Object Pool")]
    public ObjectPool petPool; // Thay vì summonPrefab

    private GameObject currentPet;

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

        // Nếu bạn có animator, có thể set trigger ở đây
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

        SkillBase.isSkillPlaying = false;
    }
}
