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

    private bool isTemporarilyDisabled = false;

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
        if (isDead || !agent.isOnNavMesh || isTemporarilyDisabled) return;

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
                ResetChase();
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
        if (isDead || isChasing || isInAlarmState || alarmTriggered)
            return;

        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
        if (hits.Length > 0)
        {
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

    void ResetChase()
    {
        isChasing = false;
        isInAlarmState = false;
        alarmTriggered = false;
        player = null;

        agent.speed = patrolSpeed;
        enemyAttack.SetTarget(null);
        GoToNextPatrolPoint();
    }

    public void OnAlarmFinished()
    {
        isInAlarmState = false;

        if (player != null)
        {
            isChasing = true;
            agent.speed = chaseSpeed;
            agent.isStopped = false;

            if (agent.isOnNavMesh)
                agent.SetDestination(player.position);
        }
        else
        {
            ResetChase();
        }
    }

    public void ResumePatrolAfterHit()
    {
        if (isDead) return;
        ResetChase();
    }

    public void CancelAlarm()
    {
        isInAlarmState = false;
        isChasing = true;
        agent.isStopped = false;

        if (player != null)
        {
            agent.speed = chaseSpeed;

            if (agent.isOnNavMesh)
                agent.SetDestination(player.position);
        }
    }

    public void SetTemporarilyDisabled(bool state)
    {
        isTemporarilyDisabled = state;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, lostPlayerDistance);
    }
}
