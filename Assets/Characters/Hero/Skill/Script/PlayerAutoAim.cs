using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
public class PlayerAutoAim : MonoBehaviour
{
    [Header("Lock-on Settings")]
    public float lockRange = 10f;
    public float maxLockAngle = 60f;
    public LayerMask enemyLayer;
    public Transform aimPivot;
    public float targetYOffset = 1.0f;

    private Transform currentTarget;
    private InputAction lockOnAction;

    private PlayerMovement playerMovement;

    // Danh sách các enemy đã bị loại khỏi danh sách lock
    private HashSet<Transform> ignoredEnemies = new HashSet<Transform>();

    void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        lockOnAction = playerInput.actions["LockOn"];
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        lockOnAction.performed += OnLockOnPressed;
        lockOnAction.Enable();
    }

    void OnDisable()
    {
        lockOnAction.performed -= OnLockOnPressed;
        lockOnAction.Disable();
    }

    void Update()
    {
        if (currentTarget != null)
        {
            if (!currentTarget.gameObject.activeInHierarchy || 
                Vector3.Distance(transform.position, currentTarget.position) > lockRange)
            {
                TryLockNewTargetOrUnlock();
                return;
            }

            RotateTowardsTarget();
        }
    }

    private void OnLockOnPressed(InputAction.CallbackContext context)
    {
        if (currentTarget == null)
            LockTarget();
        else
            UnlockTarget();
    }

    void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector3 directionToTarget = currentTarget.position - transform.position;
        directionToTarget.y = 0;
        if (directionToTarget != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        Vector3 aimDirection = currentTarget.position + Vector3.up * targetYOffset - aimPivot.position;
        aimDirection.y = 0;
        aimPivot.forward = Vector3.Lerp(aimPivot.forward, aimDirection.normalized, Time.deltaTime * 10f);
    }

    void LockTarget()
    {
        currentTarget = FindBestTarget();
        if (currentTarget != null)
        {
            playerMovement.lockOnTarget = currentTarget;
            Debug.Log("Locked onto: " + currentTarget.name);
        }
    }

    void UnlockTarget()
    {
        Debug.Log("Target unlocked");
        if (currentTarget != null)
            ignoredEnemies.Add(currentTarget);

        currentTarget = null;
        playerMovement.lockOnTarget = null;
    }

    void TryLockNewTargetOrUnlock()
    {
        Transform newTarget = FindBestTarget();
        if (newTarget != null)
        {
            currentTarget = newTarget;
            playerMovement.lockOnTarget = currentTarget;
            Debug.Log("Switched to new target: " + currentTarget.name);
        }
        else
        {
            UnlockTarget();
        }
    }

    Transform FindBestTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, lockRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var enemy in enemies)
        {
            if (!enemy.gameObject.activeInHierarchy || ignoredEnemies.Contains(enemy.transform)) continue;

            Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (angle < maxLockAngle && distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget;
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }

    // Gọi từ enemy khi chết
    public void OnEnemyDied(Transform enemy)
    {
        if (currentTarget == enemy)
        {
            Debug.Log("Locked-on enemy died");
            TryLockNewTargetOrUnlock();
        }

        ignoredEnemies.Add(enemy);
    }
}
