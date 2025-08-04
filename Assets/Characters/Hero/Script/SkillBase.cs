using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public abstract class SkillBase : MonoBehaviour
{
    [Header("Cooldown Settings")]
    public float cooldown = 2f;
    protected bool isOnCooldown = false;

    [Header("Skill State")]
    public static bool isSkillPlaying { get; protected set; } = false;
    public bool isInterruptible = true;

    // References
    protected PlayerMovement movement;
    protected Animator animator;
    protected PlayerAutoAim autoAim;

    protected virtual void Awake()
    {
        // Dùng Awake để đảm bảo lấy được component sớm
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        autoAim = GetComponent<PlayerAutoAim>();
    }

    public void TryUseSkill()
    {
        if (isOnCooldown || isSkillPlaying || movement == null || !movement.IsGrounded || !movement.IsCombat)
            return;

        isSkillPlaying = true;
        Activate();
        StartCoroutine(CooldownRoutine());
    }

    protected abstract void Activate();

    protected virtual IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    /// <summary>
    /// Gọi khi bị đánh để ngắt kỹ năng giữa chừng (nếu cho phép).
    /// </summary>
    public virtual void InterruptSkill()
    {
        if (!isSkillPlaying || !isInterruptible) return;

        Debug.LogWarning($"{gameObject.name} - Skill interrupted!");

        EndSkill(); // Mặc định kết thúc
    }

    /// <summary>
    /// Gọi ở cuối animation hoặc logic skill để reset trạng thái.
    /// </summary>
    public virtual void EndSkill()
    {
        isSkillPlaying = false;

        if (movement != null)
            movement.isMovementLocked = false;
    }
}
