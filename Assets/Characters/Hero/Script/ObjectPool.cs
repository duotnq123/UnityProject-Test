using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    [Header("Pooling Settings")]
    public GameObject prefab;
    public int initialSize = 10;
    public bool autoExpand = true;

    private Queue<GameObject> pool = new Queue<GameObject>();

    private void Awake()
    {
        if (prefab == null)
        {
            Debug.LogError($"[{gameObject.name}] Missing prefab reference!");
            return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            CreateAndAddToPool();
        }
    }

    private GameObject CreateAndAddToPool()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// Lấy object từ pool và kích hoạt nó
    /// </summary>
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;

        // Lấy từ pool nếu có
        while (pool.Count > 0)
        {
            obj = pool.Dequeue();

            // Nếu object đã bị Destroy, bỏ qua
            if (obj == null)
                continue;

            break;
        }

        // Nếu không lấy được object hợp lệ
        if (obj == null)
        {
            if (autoExpand)
            {
                obj = CreateAndAddToPool();
                pool.Dequeue(); // bỏ phần tử mới tạo ra khỏi hàng đợi để dùng
            }
            else
            {
                Debug.LogWarning($"[{gameObject.name}] Pool exhausted and autoExpand is off!");
                return null;
            }
        }

        if (obj != null)
        {
            obj.transform.SetParent(null);
            obj.transform.SetPositionAndRotation(position, rotation);
            obj.SetActive(true);

            // Reset particle nếu có
            var psArray = obj.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var ps in psArray)
            {
                ps.Clear(true);
                ps.Play(true);
            }
        }

        return obj;
    }

    /// <summary>
    /// Trả object về pool và ẩn nó đi
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        if (!pool.Contains(obj)) // tránh double enqueue
        {
            obj.SetActive(false);
            obj.transform.SetParent(transform);
            pool.Enqueue(obj);
        }
    }
}
