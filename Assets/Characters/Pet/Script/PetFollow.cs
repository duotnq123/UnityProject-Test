using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PetFollow : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform player;
    public float followDistance = 2f;
    public float walkSpeed = 2f;
    public float runSpeed = 4f;
    public float rotationSpeed = 5f;
    public float stopDistanceBuffer = 0.1f;
    public float minTurnAngle = 5f;

    private Animator animator;
    private Vector3 previousPosition;

    void Start()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }

    void Update()
    {
        Move();
        AnimateSpeed();
    }

    void Move()
    {
        if (!player) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (distance > followDistance + stopDistanceBuffer)
        {
            float currentSpeed = PlayerMovement.IsPlayerRunning ? runSpeed : walkSpeed;
            Vector3 moveDir = direction.normalized;

            // Xoay theo hướng di chuyển
            if (Vector3.Angle(transform.forward, moveDir) > minTurnAngle)
            {
                Quaternion targetRot = Quaternion.LookRotation(moveDir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
            }

            // Di chuyển theo XZ
            Vector3 targetPos = player.position - moveDir * followDistance;
            targetPos.y = transform.position.y; // giữ nguyên Y
            Vector3 movePos = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
            transform.position = new Vector3(movePos.x, transform.position.y, movePos.z);
        }
    }

    void AnimateSpeed()
    {
        float actualSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed, 0.1f, Time.deltaTime);
        previousPosition = transform.position;
    }
}
