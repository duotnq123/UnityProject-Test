using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    [Header("References")]
    public Slider slider;
    private Canvas canvas;

    [Header("Follow Settings")]
    public Vector3 offset = new Vector3(0, 2f, 0);
    private Transform target;

    public void Initialize(Transform targetTransform)
    {
        target = targetTransform;

        if (slider == null)
            slider = GetComponentInChildren<Slider>();

        if (canvas == null)
            canvas = GetComponentInChildren<Canvas>(); 
    }

    public void UpdateHealth(float current, float max)
    {
        if (slider != null)
            slider.value = current / max;
    }

    void LateUpdate()
    {
        if (target == null || Camera.main == null) return;

        transform.position = target.position + offset;
        transform.forward = Camera.main.transform.forward;
    }
}
