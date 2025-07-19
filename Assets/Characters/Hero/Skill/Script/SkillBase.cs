using UnityEngine;
using System.Collections;


public abstract class SkillBase : MonoBehaviour
{
    public float cooldown = 2f;
    public static bool isSkillPlaying = false;
    protected bool isOnCooldown = false;

    protected PlayerMovement movement;
    protected Animator animator;
    protected PlayerAutoAim autoAim;

    protected virtual void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        autoAim = GetComponent<PlayerAutoAim>();
    }

    public void TryUseSkill()
    {
        if (isOnCooldown || isSkillPlaying || movement == null || !movement.IsGrounded)
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
}
