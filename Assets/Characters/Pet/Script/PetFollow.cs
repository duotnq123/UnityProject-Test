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

    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;
    public float gravity = -20f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;

    private Animator animator;
    private Vector3 previousPosition;
    private float verticalVelocity;
    private bool isGrounded;
    void Start()
    {
        animator = GetComponent<Animator>();
        previousPosition = transform.position;
    }
    void Update()
    {
        GroundCheck();
        ApplyGravity();
        Move();
        AnimateSpeed();
    }
    void Move()
    {
        if (!player) return;

        Vector3 direction = player.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;
        float currentSpeed = PlayerMovement.IsPlayerRunning ? runSpeed : walkSpeed;
        Vector3 moveDir = direction.normalized;
       if (distance > followDistance + stopDistanceBuffer)
       {
           if (Vector3.Angle(transform.forward, moveDir) > minTurnAngle)
           {
               Quaternion targetRot = Quaternion.LookRotation(moveDir);
               transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
           }

           Vector3 targetPosition = player.position - moveDir * followDistance;
           targetPosition.y= transform.position.y;

           Vector3 horizontalMove = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
           transform.position = new Vector3(horizontalMove.x, transform.position.y, horizontalMove.z);
       }
        transform.position += new Vector3(0f, verticalVelocity * Time.deltaTime, 0f);
    }
    void ApplyGravity()
    {
        if (!isGrounded)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        if (IsTouchingGround() && verticalVelocity <= 0f)
        {
            transform.position = new Vector3(transform.position.x, GetGroundHeight(), transform.position.z);
            verticalVelocity = 0f;
            isGrounded = true;
            animator.SetBool("isGrounded", true);
            animator.SetTrigger("Land");
        }
    }
    void GroundCheck()
    {
        isGrounded = IsTouchingGround();
        animator.SetBool("isGrounded", isGrounded);
    }

    bool IsTouchingGround()
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }

    float GetGroundHeight()
    {
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, Vector3.down, out RaycastHit hit, 2f, groundLayer))
        {
            return hit.point.y;
        }
        return transform.position.y;
    }
    void AnimateSpeed()
    {
        float actualSpeed = (transform.position - previousPosition).magnitude / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed, 0.1f, Time.deltaTime);
        previousPosition = transform.position;
    }
    public void Jump()
    {
        if (!isGrounded) return;


        verticalVelocity = Mathf.Sqrt(-2f * gravity * jumpHeight);
        isGrounded = false;

        animator.SetBool("isGrounded", false);
        animator.SetTrigger("Jump");
    }
}
