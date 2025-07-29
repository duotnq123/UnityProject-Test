using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class PetAI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private Transform currentTarget;

    [Header("Combat")]
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    private float lastAttackTime;

    [Header("Movement")]
    public float moveSpeed = 5f;

    void OnEnable()
    {
    agent.speed = moveSpeed;
    FindNewTarget();
}


    private Vector3 previousPosition;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }
    void Update()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            FindNewTarget();
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.position);

        if (dist <= attackRange)
        {
            agent.isStopped = true;

            if (Time.time - lastAttackTime > attackCooldown)
            {
                animator.SetTrigger("Attack");
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(currentTarget.position);
        }

        UpdateAnimation();
    }

    void UpdateAnimation()
    {
        float actualSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed, 0.1f, Time.deltaTime);
        previousPosition = transform.position;
    }

    void FindNewTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDist = Mathf.Infinity;
        Transform closest = null;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy.transform;
            }
        }

        currentTarget = closest;
    }
}
