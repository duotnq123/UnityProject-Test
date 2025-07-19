using UnityEngine;
using System.Collections;

public class AutoReturnParticle : MonoBehaviour
{
    private ParticleSystem ps;
    private ObjectPool pool;

    void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        pool = FindObjectOfType<ObjectPool>(); // Hoặc gán trong Inspector
    }

    void OnEnable()
    {
        if (ps != null)
            StartCoroutine(ReturnAfterParticle());
    }

    private IEnumerator ReturnAfterParticle()
    {
        // Chờ cho đến khi ParticleSystem hoàn thành
        yield return new WaitForSeconds(ps.main.duration);
        if (pool != null)
            pool.ReturnObject(gameObject);
    }
}