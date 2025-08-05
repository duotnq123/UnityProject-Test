using UnityEngine;
using System.Collections;

public enum SkillType { Skill1, Skill2, Skill3, Skill4, Skill5 }

[RequireComponent(typeof(Animator))]
public abstract class SkillBase : MonoBehaviour
{
    [Header("Cooldown Settings")]
    public float cooldown = 2f;
    protected bool isOnCooldown = false;
    protected float cooldownTimer = 0f; // <- thêm dòng này

    [Header("Mana Settings")]
    public float manaCost = 20f;

    [Header("Skill State")]
    public static bool isSkillPlaying { get; protected set; } = false;
    public bool isInterruptible = true;

    [Header("Skill Info")]
    public SkillType skillType;

    // References
    protected PlayerMovement movement;
    protected Animator animator;
    protected PlayerAutoAim autoAim;
    protected PlayerMana playerMana;

    // ✅ Public getter cho UI
    public bool IsOnCooldown => isOnCooldown;
    public float GetCooldownLeft() => Mathf.Max(0f, cooldown - cooldownTimer);

    protected virtual void Awake()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        autoAim = GetComponent<PlayerAutoAim>();
        playerMana = GetComponent<PlayerMana>();
    }

    public void TryUseSkill()
    {
        if (isOnCooldown || isSkillPlaying || movement == null || !movement.IsGrounded || !movement.IsCombat)
            return;

        if (playerMana != null && !playerMana.UseMana(manaCost))
        {
            Debug.Log("Not enough mana!");
            return;
        }

        isSkillPlaying = true;
        Activate();
        StartCoroutine(CooldownRoutine());
    }

    protected abstract void Activate();

    protected virtual IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;
        cooldownTimer = 0f;

        while (cooldownTimer < cooldown)
        {
            cooldownTimer += Time.deltaTime;
            yield return null;
        }

        isOnCooldown = false;
    }

    public virtual void InterruptSkill()
    {
        if (!isSkillPlaying || !isInterruptible) return;

        Debug.LogWarning($"{gameObject.name} - Skill interrupted!");
        EndSkill();
    }

    public virtual void EndSkill()
    {
        isSkillPlaying = false;
        if (movement != null)
            movement.isMovementLocked = false;
    }
}
