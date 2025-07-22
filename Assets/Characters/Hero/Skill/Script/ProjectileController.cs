using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 20f;

    private Vector3 startPos;
    private ObjectPool projectilePool; 
    private Vector3 direction = Vector3.forward;
    private bool isReturning = false;

    private System.Collections.IEnumerator DelayedReturnToPool(float delay)
    {
        yield return new WaitForSeconds(delay);
        ReturnToPool();
    }

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
    }

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

    void OnTriggerEnter(Collider other)
    {
        if (isReturning) return;

        Debug.Log("Projectile hit: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            StartCoroutine(DelayedReturnToPool(5f));  // Delay trả đạn
            return;
        }

        var breakable = other.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            Debug.Log("Breakable hit! Breaking...");
            breakable.Break();
            ReturnToPool();
        }
    }

    void ReturnToPool()
    {
        if (isReturning) return;
        isReturning = true;

        if (projectilePool != null)
            projectilePool.ReturnObject(gameObject);
        else
            Debug.LogWarning($"Cannot return {gameObject.name}: projectilePool is null!");
    }

    public void SetPool(ObjectPool pool)
    {
        projectilePool = pool;
    }
}
