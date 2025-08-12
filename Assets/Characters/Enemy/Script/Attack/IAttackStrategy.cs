using UnityEngine;

public interface IAttackStrategy
{
    void ExecuteAttack(Transform attackPoint, float range, int damage, LayerMask playerLayer);
}
