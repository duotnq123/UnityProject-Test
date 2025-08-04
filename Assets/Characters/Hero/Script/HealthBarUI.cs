using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider healthSlider;

    public void SetHealth(float healthNormalized)
    {
        if (healthSlider != null)
        {
            healthSlider.value = healthNormalized;
        }
    }
}
