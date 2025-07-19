using UnityEngine;
using System.Collections;

public class BreakableObject : MonoBehaviour
{
    public float respawnTime = 3f;
    public AudioClip breakSound;

    private bool isBroken = false;
    private GameObject spawnedBroken;

    private Renderer[] renderers;
    private Collider[] colliders;

    public ObjectPool breakVFXPool;        // Gán từ scene
    public ObjectPool brokenVersionPool;   // Gán từ scene

    void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>(true);
        colliders = GetComponentsInChildren<Collider>(true);
    }

    public void Break()
    {
        if (isBroken) return;
        isBroken = true;

        if (breakVFXPool)
            breakVFXPool.GetObject(transform.position, Quaternion.identity);

        if (breakSound)
            AudioSource.PlayClipAtPoint(breakSound, transform.position);

        if (brokenVersionPool)
            spawnedBroken = brokenVersionPool.GetObject(transform.position, transform.rotation);

        SetObjectVisible(false);
        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnTime);

        if (spawnedBroken && brokenVersionPool)
            brokenVersionPool.ReturnObject(spawnedBroken);

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
