using UnityEngine;

public interface IHealthObserver
{
    void OnHealthChanged(float current, float max);
    void OnDeath();
}
