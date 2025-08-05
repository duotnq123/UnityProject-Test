using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    public Slider staminaSlider;
    public PlayerStamina playerStamina;

    void Start()
    {
        playerStamina.onStaminaChanged.AddListener(UpdateBar);
        UpdateBar();
    }

    void UpdateBar()
    {
        staminaSlider.value = playerStamina.GetStaminaPercent();
    }
}
