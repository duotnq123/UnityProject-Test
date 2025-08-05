using UnityEngine;
using UnityEngine.Events;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float regenRate = 10f;

    [Header("Events")]
    public UnityEvent onStaminaChanged;

    void Start()
    {
        currentStamina = maxStamina;
    }

    void Update()
    {
        RegenStamina();
    }

    void RegenStamina()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
            currentStamina = Mathf.Min(currentStamina, maxStamina);
            onStaminaChanged?.Invoke();
        }
    }

    public bool UseStamina(float amount)
    {
        if (currentStamina >= amount)
        {
            currentStamina -= amount;
            onStaminaChanged?.Invoke();
            return true;
        }
        return false;
    }

    public float GetStaminaPercent()
    {
        return currentStamina / maxStamina;
    }
}
