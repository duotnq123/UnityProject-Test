using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 20f;
    public float delayBeforeReturn = 5f;

    private Vector3 startPos;
    private Vector3 direction = Vector3.forward;
    private bool isReturning = false;

    private ObjectPool projectilePool;
    private ObjectPool explosionPool; // <-- được gán từ Skill

    void OnEnable()
    {
        startPos = transform.position;
        isReturning = false;
    }

    void Update()
    {
        if (isReturning) return;

        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float distanceTraveled = Vector3.Distance(startPos, transform.position);
        if (distanceTraveled > maxDistance)
        {
            ReturnToPool();
        }
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

    public void SetPool(ObjectPool pool)
    {
        projectilePool = pool;
    }

    public void SetExplosionPool(ObjectPool pool)
    {
        explosionPool = pool;

        // Gán pool này vào ProjectileDamage
        var damageComp = GetComponent<ProjectileDamage>();
        if (damageComp != null)
        {
            damageComp.SetExplosionPool(pool);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isReturning) return;

        var breakable = other.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.Break();
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (projectilePool != null)
            projectilePool.ReturnObject(gameObject);
        else
            Destroy(gameObject);
    }
}
