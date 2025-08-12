using UnityEngine;

public interface IHealth
{
    void TakeDamage(float amount);
    void ResetHealth();
    bool IsDead { get; }
}
