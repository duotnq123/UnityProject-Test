using UnityEngine;
using UnityEngine.Events;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("References")]
    private Animator animator;

    [Header("Events")]
    public UnityEvent onDeath;
    public UnityEvent<float> onHealthChanged;

    [SerializeField] private HealthBarUI healthBarUI;

    private Coroutine stunCoroutine;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Cập nhật UI lần đầu
        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        // Update UI
        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth / maxHealth);

        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (currentHealth > 0f)
        {
            InterruptCurrentSkill();
            animator.SetTrigger("HitReact");
            StunPlayer(0.4f); // Choáng nhẹ 0.4 giây
        }
        else
        {
            Die();
        }
        if (currentHealth > 0f)
        {
            InterruptCurrentSkill();  // skill nào cho phép mới bị ngắt
            animator.SetTrigger("HitReact");
            StunPlayer(0.4f);         // stun nếu skill cho phép
        }

    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        onHealthChanged?.Invoke(currentHealth / maxHealth);

        if (healthBarUI != null)
            healthBarUI.SetHealth(currentHealth / maxHealth);
    }

    private void Die()
    {
        isDead = true;
        currentHealth = 0f;

        animator.SetTrigger("Death");

        onDeath?.Invoke();

        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.enabled = false;

        PlayerSkillManager skillManager = GetComponent<PlayerSkillManager>();
        if (skillManager != null) skillManager.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
    }

    private void StunPlayer(float duration)
    {
        SkillBase[] skills = GetComponents<SkillBase>();

        // Nếu có skill đang dùng và KHÔNG bị ngắt → không stun
        foreach (var skill in skills)
        {
            if (SkillBase.isSkillPlaying && !skill.isInterruptible)
                return;
        }

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }


    private IEnumerator StunRoutine(float duration)
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        PlayerSkillManager skillManager = GetComponent<PlayerSkillManager>();

        if (movement != null) movement.enabled = false;
        if (skillManager != null) skillManager.enabled = false;

        yield return new WaitForSeconds(duration);

        if (!isDead)
        {
            if (movement != null) movement.enabled = true;
            if (skillManager != null) skillManager.enabled = true;
        }

        stunCoroutine = null;
    }
    private void InterruptCurrentSkill()
    {
        SkillBase[] skills = GetComponents<SkillBase>();
        foreach (var skill in skills)
        {
            skill.InterruptSkill();
        }
    }
    public bool IsFullHealth()
    {
        return currentHealth >= maxHealth;
    }
}
