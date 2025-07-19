using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    private Queue<GameObject> pool = new Queue<GameObject>();
    public GameObject prefab; // Prefab để tạo đối tượng
    public int initialSize = 10; // Kích thước ban đầu của pool
    public bool autoExpand = true; // Tự động mở rộng pool nếu cần

    void Start()
    {
        // Kiểm tra và khởi tạo pool với số lượng đối tượng ban đầu
        if (prefab == null)
        {
            Debug.LogError($"No prefab assigned to {gameObject.name}!");
            return;
        }
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.transform.SetParent(null);
            Debug.Log($"Retrieved {obj.name} from {gameObject.name} at {position}");
        }
        else if (autoExpand)
        {
            obj = Instantiate(prefab, position, rotation);
            Debug.Log($"Created new {obj.name} for {gameObject.name} at {position}");
        }
        else
        {
            Debug.LogWarning($"Pool {gameObject.name} is empty and autoExpand is disabled!");
            return null;
        }

        // Xử lý ParticleSystem và các hệ thống con
        ParticleSystem[] particleSystems = obj.GetComponentsInChildren<ParticleSystem>();
        if (particleSystems.Length > 0)
        {
            foreach (var ps in particleSystems)
            {
                ps.Clear();
                ps.Play();
                Debug.Log($"Playing ParticleSystem on {obj.name}");
            }
        }

        return obj;
    }

    public void ReturnObject(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogWarning($"Attempted to return null object to {gameObject.name}!");
            return;
        }
        obj.SetActive(false);
        pool.Enqueue(obj);
        Debug.Log($"Returned {obj.name} to {gameObject.name}");
    }
}