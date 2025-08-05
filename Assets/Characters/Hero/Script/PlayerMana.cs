using UnityEngine;
using UnityEngine.Events;

public class PlayerMana : MonoBehaviour
{
    [Header("Mana Settings")]
    public float maxMana = 100f;
    public float currentMana;
    public float regenRate = 5f;

    [Header("Events")]
    public UnityEvent onManaChanged;

    void Start()
    {
        currentMana = maxMana;
    }

    void Update()
    {
        RegenMana();
    }

    void RegenMana()
    {
        if (currentMana < maxMana)
        {
            currentMana += regenRate * Time.deltaTime;
            currentMana = Mathf.Min(currentMana, maxMana);
            onManaChanged?.Invoke();
        }
    }

    public bool UseMana(float amount)
    {
        if (currentMana >= amount)
        {
            currentMana -= amount;
            onManaChanged?.Invoke();
            return true;
        }
        return false;
    }

    public float GetManaPercent()
    {
        return currentMana / maxMana;
    }
}
