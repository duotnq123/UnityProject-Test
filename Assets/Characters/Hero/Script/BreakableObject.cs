using UnityEngine;
using System.Collections;

public class BreakableObject : MonoBehaviour
{
    [Header("Break Settings")]
    public float respawnTime = 3f;
    public AudioClip breakSound;

    [Header("Pool References")]
    public ObjectPool breakVFXPool;        // Pool cho hiệu ứng vỡ
    public ObjectPool brokenVersionPool;   // Pool cho bản object bị vỡ

    private bool isBroken = false;
    private GameObject spawnedBroken;

    private Renderer[] renderers;
    private Collider[] colliders;

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
    }

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        // Hiệu ứng vỡ
        if (breakVFXPool)
            breakVFXPool.GetObject(transform.position, Quaternion.identity);

        // Âm thanh vỡ
        if (breakSound)
            AudioSource.PlayClipAtPoint(breakSound, transform.position);

        // Spawn phiên bản vỡ
        if (brokenVersionPool)
            spawnedBroken = brokenVersionPool.GetObject(transform.position, transform.rotation);

        // Ẩn object gốc (chỉ tắt renderer + collider, KHÔNG setActive)
        SetObjectVisible(false);

        // Bắt đầu đếm thời gian để respawn
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        // Trả lại object vỡ vào pool
        if (spawnedBroken && brokenVersionPool)
            brokenVersionPool.ReturnObject(spawnedBroken);

        // Hiện lại object ban đầu
        SetObjectVisible(true);
        isBroken = false;
    }

    private void SetObjectVisible(bool visible)
    {
        foreach (var r in renderers)
            r.enabled = visible;

        foreach (var c in colliders)
            c.enabled = visible;
    }
}
