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

    public bool isInAlarmState = false;
    private bool alarmTriggered = false;

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

        if (isInAlarmState)
        {
            agent.isStopped = true;
            return;
        }

        if (isChasing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > lostPlayerDistance)
            {
                // Player thoát khỏi tầm -> dừng chase và cho phép alarm lại nếu phát hiện mới
                isChasing = false;
                isInAlarmState = false;
                alarmTriggered = false;
                player = null;

                agent.speed = patrolSpeed;
                enemyAttack.SetTarget(null);
                GoToNextPatrolPoint();
                return;
            }

            if (distance <= enemyAttack.attackRange)
            {
                agent.SetDestination(transform.position);
                agent.isStopped = true;
                enemyAttack.SetTarget(player);
            }
            else
            {
                agent.isStopped = false;
                agent.SetDestination(player.position);
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
            // Nếu đã trigger alarm rồi thì không làm lại nữa
            if (alarmTriggered) return;

            player = hits[0].transform;

            isInAlarmState = true;
            alarmTriggered = true;
            agent.speed = chaseSpeed;

            animator.SetTrigger("Alarm");
            animator.SetBool("IsCombo", false);
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

    public void OnAlarmFinished()
    {
        isInAlarmState = false;

        if (player != null)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            // Không còn player → quay lại patrol
            isChasing = false;
            agent.speed = patrolSpeed;
            agent.isStopped = false;
            GoToNextPatrolPoint();
        }
    }

    public void ResumePatrolAfterHit()
    {
        if (isDead) return;

        isChasing = false;
        player = null;
        alarmTriggered = false;
        isInAlarmState = false;

        agent.speed = patrolSpeed;
        agent.isStopped = false;
        GoToNextPatrolPoint();
    }

    public void CancelAlarm()
    {
        isInAlarmState = false;
        agent.isStopped = false;
        isChasing = true;

        if (player != null)
        {
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
    }
}
