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
    public PlayerAutoAim autoAim;

    private Animator animator;
    private PlayerMovement movement;

    private int comboStep = 0;
    public bool isAttacking = false;
    private bool canCombo = false;
    private bool queuedNextAttack = false;
    private bool isCombat = false;
    private float lastClickTime = 0f;

    [Header("Combo Timing")]
    public float comboResetTime = 2f;

    [Header("Stamina")]
    public float staminaCostPerAttack = 15f;
    private PlayerStamina playerStamina;

    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        playerStamina = GetComponent<PlayerStamina>();
    }

    void Update()
    {
        if (SkillBase.isSkillPlaying) return;

        isCombat = animator.GetBool("IsCombat");

        if (Time.time - lastClickTime > comboResetTime && comboStep > 0)
        {
            ResetCombo();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleAttackInput();
        }
    }

    void HandleAttackInput()
    {
        if (!isCombat) return;

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
        if (comboStep == 0 && movement != null && !movement.IsGrounded)
            return;

        if (playerStamina != null && !playerStamina.UseStamina(staminaCostPerAttack))
        {
            return;
        }

        comboStep = (comboStep % 3) + 1;

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

    public void DisableComboWindow() => canCombo = false;

    public void CastProjectile()
    {
        Vector3 spawnPos = spellSpawnPoint.position;
        Vector3 direction = spellSpawnPoint.forward;
        Quaternion rotation = spellSpawnPoint.rotation;

        Transform target = autoAim?.GetTarget();
        if (target != null)
        {
            Vector3 targetPosition = target.GetComponent<Collider>()?.bounds.center ?? target.position;
            direction = (targetPosition - spawnPos).normalized;
            rotation = Quaternion.LookRotation(direction);
        }

        ObjectPool selectedPool = comboStep switch
        {
            1 => spell1Pool,
            2 => spell2Pool,
            3 => spell3Pool,
            _ => null
        };

        if (selectedPool == null) return;

        GameObject projectile = selectedPool.GetObject(spawnPos, rotation);
        if (projectile != null)
        {
            var controller = projectile.GetComponent<ProjectileController>();
            if (controller != null)
            {
                controller.SetDirection(direction);
                controller.SetPool(selectedPool);
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
