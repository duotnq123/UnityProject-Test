using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProjectileController : MonoBehaviour
{
    public float speed = 10f;
    public float maxDistance = 20f;

    private Vector3 startPos;
    private ObjectPool projectilePool; 
    private Vector3 direction = Vector3.forward;

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
        startPos = transform.position; // Cập nhật vị trí bắt đầu khi tái sử dụng
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        float distanceTraveled = Vector3.Distance(startPos, transform.position);
        if (distanceTraveled > maxDistance)
        {
            ReturnToPool();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Projectile hit: " + other.name);

        if (other.CompareTag("Enemy"))
        {
            // TODO: Gây sát thương ở đây nếu cần
            if (projectilePool != null)
                projectilePool.ReturnObject(gameObject);
            else
                Debug.LogWarning($"Cannot return {gameObject.name} on Enemy hit: projectilePool is null!");
            return;
        }

        var breakable = other.GetComponent<BreakableObject>();
        if (breakable != null)
        {
            Debug.Log("Breakable hit! Breaking...");
            breakable.Break();
            if (projectilePool != null)
                projectilePool.ReturnObject(gameObject);
            else
                Debug.LogWarning($"Cannot return {gameObject.name} on Breakable hit: projectilePool is null!");
        }
    }
    void ReturnToPool()
    {
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