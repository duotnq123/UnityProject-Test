using UnityEngine;

public class MageCombo : MonoBehaviour
{
    public Transform spellSpawnPoint;
    public ObjectPool spellPool;
    private Animator animator;

    private int comboStep = 0;
    private bool isAttacking = false;
    private float lastAttackTime;
    public float comboResetTime = 1.5f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Chuột trái
        {
            TryCastSpell();
        }

        if (Time.time - lastAttackTime > comboResetTime)
        {
            comboStep = 0;
        }
    }

    void TryCastSpell()
    {
        if (isAttacking) return;

        comboStep++;
        if (comboStep > 3)
            comboStep = 1;

        lastAttackTime = Time.time;

        animator.SetInteger("comboStep", comboStep);
        animator.SetTrigger("attackTrigger");
        isAttacking = true; // Khóa spam cho đến khi animation kết thúc
    }

    // Gọi từ Animation Event
    public void CastProjectile()
    {
        if (spellPool != null && spellSpawnPoint != null)
        {
            spellPool.GetObject(spellSpawnPoint.position, spellSpawnPoint.rotation);
        }
    }

    // Gọi từ Animation Event ở cuối Cast1, 2, 3
    public void EndAttack()
    {
        isAttacking = false;
    }
}
