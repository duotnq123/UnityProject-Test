using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Animator))]
public class PlayerSkill : MonoBehaviour
{
    [Header("Skill Settings")]
    public float skillCooldown = 2f;

    [Header("VFX")]
    public GameObject skillVFXPrefab;
    public Transform skillVFXSpawnPoint;
    public GameObject skillAuraVFXPrefab;
    public Transform skillAuraSpawnPoint;

    private PlayerMovement movement;
    private Animator animator;
    private bool isUsingSkill = false;
    private GameObject activeAuraVFX;
    private ObjectPool skillVFXPool; // Pool cho skill VFX
    private ObjectPool skillAuraPool; // Pool cho aura VFX

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        animator = GetComponent<Animator>();
        // Tìm các ObjectPool trong scene (gán trong Inspector)
        skillVFXPool = GameObject.Find("SkillVFXPool")?.GetComponent<ObjectPool>();
        skillAuraPool = GameObject.Find("SkillAuraPool")?.GetComponent<ObjectPool>();
    }

    public void SkillInputHandler(InputAction.CallbackContext context)
    {
        if (context.performed && !isUsingSkill)
        {
            animator.SetTrigger("skill");
            StartCoroutine(SkillCooldownRoutine());
        }
    }

    private IEnumerator SkillCooldownRoutine()
    {
        isUsingSkill = true;
        movement.isMovementLocked = true;

        yield return new WaitForSeconds(skillCooldown);
        // Việc mở khoá sẽ do OnSkillEnd handle bằng animation event
    }

    // Gọi bởi Animation Event đầu animation
    public void ShowAura()
    {
        if (skillAuraPool != null && skillAuraVFXPrefab != null && skillAuraSpawnPoint != null)
        {
            activeAuraVFX = skillAuraPool.GetObject(skillAuraSpawnPoint.position, skillAuraSpawnPoint.rotation);
            activeAuraVFX.transform.SetParent(transform);
        }
    }

    // Gọi bởi Animation Event giữa animation (tung skill)
    public void FireProjectile()
    {
        PlaySkillVFX();
        TryBreakObject();
    }

    // Gọi bởi Animation Event cuối animation
    public void OnSkillEnd()
    {
        if (activeAuraVFX != null && skillAuraPool != null)
        {
            skillAuraPool.ReturnObject(activeAuraVFX);
            activeAuraVFX = null;
        }

        isUsingSkill = false;
        movement.isMovementLocked = false;
    }

    private void PlaySkillVFX()
    {
        if (skillVFXPool != null && skillVFXPrefab != null && skillVFXSpawnPoint != null)
        {
            skillVFXPool.GetObject(skillVFXSpawnPoint.position, transform.rotation);
        }
    }

    private void TryBreakObject()
    {
        Ray ray = new Ray(skillVFXSpawnPoint.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 3f))
        {
            var breakable = hit.collider.GetComponent<BreakableObject>();
            if (breakable != null)
            {
                breakable.Break();
            }
        }
    }
}