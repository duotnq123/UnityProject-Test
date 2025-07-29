using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    public float lifetime = 2f;
    public bool autoReturn = true;
    private ObjectPool pool;

    public void SetPool(ObjectPool p) => pool = p;

    void OnEnable()
    {
        CancelInvoke();
        if (autoReturn)
            Invoke(nameof(ReturnToPool), lifetime);
    }

    public void ReturnNow()
    {
        ReturnToPool();
    }

    void ReturnToPool()
    {
        if (pool != null)
            pool.ReturnObject(gameObject);
        else
            Destroy(gameObject);
    }
}
