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
            GameObject obj = Instantiate(prefab, transform);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    /// <summary>
    /// Lấy object từ pool và kích hoạt nó
    /// </summary>
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = null;

        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else if (autoExpand)
        {
            obj = Instantiate(prefab, transform);
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] Pool exhausted and autoExpand is off!");
            return null;
        }

        obj.transform.SetParent(null); // đảm bảo nó không bị parent làm lệch scale/position
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        // Reset particle system nếu có
        var psArray = obj.GetComponentsInChildren<ParticleSystem>(true);
        foreach (var ps in psArray)
        {
            ps.Clear(true);
            ps.Play(true);
        }

        return obj;
    }

    /// <summary>
    /// Trả object về pool và ẩn nó đi
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        obj.SetActive(false);
        obj.transform.SetParent(transform); // để gọn trong hierarchy
        pool.Enqueue(obj);
    }
}
