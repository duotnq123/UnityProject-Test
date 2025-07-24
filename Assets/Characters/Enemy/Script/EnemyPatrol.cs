using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(EnemyAttack))]
public class EnemyPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float waitTime = 2f;
    public float patrolSpeed = 1.5f;
    public float chaseSpeed = 3.0f;

    [Header("Detection Settings")]
    public float detectionRadius = 8f;
    public float lostPlayerDistance = 12f;
    public LayerMask playerLayer;

    private int currentPointIndex = 0;
    private float waitTimer = 0f;
    private NavMeshAgent agent;
    private Animator animator;
    private EnemyAttack enemyAttack;
    private Transform player;

    private bool isChasing = false;
    [HideInInspector] public bool isDead = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttack>();

        agent.speed = patrolSpeed;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[currentPointIndex].position);
    }

    void Update()
    {
        if (isDead) return;

        animator.SetFloat("Speed", agent.velocity.magnitude);

        DetectPlayer();

        if (isChasing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            // Nếu quá xa thì hủy chase và quay lại patrol
            if (distance > lostPlayerDistance)
            {
                isChasing = false;
                player = null;
                agent.speed = patrolSpeed;
                enemyAttack.SetTarget(null);
                GoToNextPatrolPoint();
                return;
            }

            // Nếu trong tầm tấn công
            if (distance <= enemyAttack.attackRange)
            {
                agent.SetDestination(transform.position); // Đứng yên
                agent.isStopped = true;
                enemyAttack.SetTarget(player);
                return;
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position); // Tiếp tục đuổi
            }

            return;
        }

        Patrol();
    }

    void DetectPlayer()
    {
        if (isDead) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
            player = hits[0].transform;

            if (!isChasing)
            {
                isChasing = true;
                agent.speed = chaseSpeed;
            }
        }
    }

    void Patrol()
    {
        if (agent.pathPending || agent.remainingDistance > 0.3f)
            return;

        waitTimer += Time.deltaTime;

        if (waitTimer >= waitTime)
        {
            GoToNextPatrolPoint();
        }
    }

    void GoToNextPatrolPoint()
    {
        if (patrolPoints.Length == 0) return;

        currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        agent.SetDestination(patrolPoints[currentPointIndex].position);
        waitTimer = 0f;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lostPlayerDistance);
    }
}
