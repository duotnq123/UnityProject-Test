using UnityEngine;
using UnityEngine.InputSystem;

public class MageComboAttack : MonoBehaviour
{
    [Header("Projectile Pools")]
    public ObjectPool spell1Pool;
    public ObjectPool spell2Pool;
    public ObjectPool spell3Pool;

    [Header("Projectile Spawn Point")]
    public Transform spellSpawnPoint;

    [Header("Auto Aim")]
    public PlayerAutoAim autoAim; // Gán trực tiếp component này trong Inspector

    private Animator animator;
    private PlayerMovement movement;

    private int comboStep = 0;
    private bool isAttacking = false;
    private bool canCombo = false;
    private bool queuedNextAttack = false;

    private float lastClickTime = 0f;
    public float comboResetTime = 2f;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Reset combo nếu quá thời gian
        if (Time.time - lastClickTime > comboResetTime && comboStep > 0)
        {
            ResetCombo();
        }

        // Tấn công bằng chuột trái
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleAttackInput();
        }
    }

    void HandleAttackInput()
    {
        lastClickTime = Time.time;

        if (isAttacking)
        {
            if (canCombo)
                StartNextComboStep();
            else
                queuedNextAttack = true;
        }
        else
        {
            StartNextComboStep();
        }
    }

    void StartNextComboStep()
    {
        // Không cho combo đầu tiên khi đang trên không
        if (comboStep == 0 && movement != null && !movement.IsGrounded)
            return;

        comboStep++;
        if (comboStep > 3) comboStep = 1;

        animator.SetInteger("comboStep", comboStep);
        animator.SetTrigger("attackTrigger");

        isAttacking = true;
        canCombo = false;
        queuedNextAttack = false;

        if (movement != null)
        {
            movement.IsMovementLocked = true;
            movement.AllowMove = false;
            movement.AllowRotation = true;
        }
    }

    public void EnableComboWindow()
    {
        canCombo = true;
        if (queuedNextAttack)
            StartNextComboStep();
    }

    public void DisableComboWindow()
    {
        canCombo = false;
    }

    public void CastProjectile()
    {
        Transform target = autoAim.GetTarget();
        Vector3 spawnPos = spellSpawnPoint.position;
        Quaternion spawnRot = spellSpawnPoint.rotation;
        Vector3 direction = spellSpawnPoint.forward;

        if (target != null)
        {
            direction = (target.position - spawnPos).normalized;
            spawnRot = Quaternion.LookRotation(direction);
        }

        GameObject projectile = null;

        switch (comboStep)
        {
            case 1:
                projectile = spell1Pool?.GetObject(spawnPos, spawnRot);
                break;
            case 2:
                projectile = spell2Pool?.GetObject(spawnPos, spawnRot);
                break;
            case 3:
                projectile = spell3Pool?.GetObject(spawnPos, spawnRot);
                break;
        }

        if (projectile != null)
        {
            ProjectileController controller = projectile.GetComponent<ProjectileController>();
            if (controller != null)
            {
                controller.SetDirection(direction);
            }
        }
    }


    public void EndAttack()
    {
        isAttacking = false;
        if (movement != null)
        {
            movement.IsMovementLocked = false;
            movement.AllowMove = true;
            movement.AllowRotation = true;
        }
    }

    public void ResetCombo()
    {
        comboStep = 0;
        isAttacking = false;
        canCombo = false;
        queuedNextAttack = false;

        animator.SetInteger("comboStep", 0);

        if (movement != null)
        {
            movement.IsMovementLocked = false;
            movement.AllowMove = true;
            movement.AllowRotation = true;
        }
    }
}
