using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 20f;
    public float delayBeforeReturn = 5f;

    private Vector3 startPos;
    private ObjectPool projectilePool;
    private Vector3 direction = Vector3.forward;
    private bool isReturning = false;

    void Awake()
    {
        if (projectilePool == null)
        {
            Debug.LogWarning($"ProjectilePool not assigned for {gameObject.name}!");
        }
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (isReturning) return;

        Debug.Log("Projectile hit: " + other.name);

        var breakable = other.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            breakable.Break();
            ReturnToPool();
        }
    }

    private IEnumerator DelayedReturnToPool(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(ActuallyReturnToPool());
    }

    private IEnumerator ActuallyReturnToPool()
    {
        yield return null; // chờ 1 frame để coroutine không lỗi nếu bị disable
        if (projectilePool != null)
            projectilePool.ReturnObject(gameObject);
        else
            Destroy(gameObject);
    }

    void ReturnToPool()
    {
        if (projectilePool != null)
            projectilePool.ReturnObject(gameObject);
        else
            Destroy(gameObject);
    }
}
