using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAutoAim : MonoBehaviour
{
    [Header("Lock-on Settings")]
    public float lockRange = 10f;
    public float maxLockAngle = 60f;
    public LayerMask enemyLayer;
    public Transform aimPivot;

    private Transform currentTarget;
    private InputAction lockOnAction;

    void Awake()
    {
        var playerInput = GetComponent<PlayerInput>();
        lockOnAction = playerInput.actions["LockOn"];
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

    private void OnLockOnPressed(InputAction.CallbackContext context)
    {
        if (currentTarget == null)
            LockTarget();
        else
            UnlockTarget();
    }

    void Update()
    {
        if (currentTarget != null)
        {
            if (Vector3.Distance(transform.position, currentTarget.position) > lockRange)
            {
                UnlockTarget();
                return;
            }

            Vector3 direction = currentTarget.position - aimPivot.position;
            direction.y = 0;
            aimPivot.forward = Vector3.Lerp(aimPivot.forward, direction.normalized, Time.deltaTime * 10f);
        }
    }

    void LockTarget()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, lockRange, enemyLayer);
        float closestDistance = Mathf.Infinity;
        Transform bestTarget = null;

        foreach (var enemy in enemies)
        {
            Vector3 dirToEnemy = (enemy.transform.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dirToEnemy);

            if (angle < maxLockAngle)
            {
                float dist = Vector3.Distance(transform.position, enemy.transform.position);
                if (dist < closestDistance)
                {
                    closestDistance = dist;
                    bestTarget = enemy.transform;
                }
            }
        }

        if (bestTarget != null)
        {
            currentTarget = bestTarget;
            GetComponent<PlayerMovement>().lockOnTarget = currentTarget; // Gán target cho movement
            Debug.Log("Locked onto: " + currentTarget.name);
        }
    }

    void UnlockTarget()
    {
        Debug.Log("Target unlocked");
        currentTarget = null;
        GetComponent<PlayerMovement>().lockOnTarget = null; // Reset target ở movement
    }

    public Transform GetTarget()
    {
        return currentTarget;
    }
}
