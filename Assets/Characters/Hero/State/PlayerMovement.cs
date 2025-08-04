using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 1.5f;
    public float gravity = -20f;

    [Header("References")]
    public Transform cameraTransform;
    public Transform lockOnTarget;

    [Header("External Movement Control")]
    [SerializeField] public bool isMovementLocked = false;
    [SerializeField] private bool allowMove = true;
    [SerializeField] private bool allowRotation = true;

    private CharacterController controller;
    private Animator animator;
    private Vector2 moveInput = Vector2.zero;
    private Vector3 velocity;
    private bool isRunning;
    public static bool IsPlayerRunning = false;
    private bool isGrounded;
    private bool previousIsCombat = false;

    public PetFollow petFollow;

    public bool IsMovementLocked
    {
        get => isMovementLocked;
        set
        {
            isMovementLocked = value;
            Debug.Log($"IsMovementLocked set to: {value}");
        }
    }

    public bool IsGrounded => isGrounded;
    public bool IsRunning => isRunning;
    public bool IsCombat => lockOnTarget != null;
    public Transform LockOnTarget => lockOnTarget;

    public bool AllowMove
    {
        get => allowMove;
        set
        {
            allowMove = value;
            Debug.Log($"AllowMove set to: {value}");
            if (!value)
            {
                moveInput = Vector2.zero;
                velocity.x = 0f;
                velocity.z = 0f;
            }
        }
    }

    public bool AllowRotation
    {
        get => allowRotation;
        set
        {
            allowRotation = value;
            Debug.Log($"AllowRotation set to: {value}");
        }
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        if (controller == null) Debug.LogError("CharacterController is missing!");
        if (animator == null) Debug.LogError("Animator is missing!");
        if (cameraTransform == null) Debug.LogError("cameraTransform is not assigned!");

        isMovementLocked = false;
        allowMove = true;
        allowRotation = true;
        animator.SetBool("IsCombat", false);
        previousIsCombat = false;
    }

    void Update()
    {
        if (isMovementLocked)
        {
            StopMovement();
            return;
        }

        HandleGroundCheck();
        HandleMovement();
        ApplyGravity();
        UpdateAnimator(moveInput.magnitude);
    }

    void StopMovement()
    {
        moveInput = Vector2.zero;
        velocity = Vector3.zero;
        controller.Move(Vector3.zero);
        animator.SetFloat("Speed", 0f);
        animator.SetBool("IsRunning", false);
    }

    void HandleGroundCheck()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    void HandleMovement()
    {
        if (!allowMove || moveInput == Vector2.zero) return;

        Vector3 moveDirection = GetMovementDirection();
        float speed = (isRunning && moveInput.magnitude > 0.1f) ? runSpeed : walkSpeed;
        Vector3 horizontalMove = moveDirection.normalized * speed;
        controller.Move(horizontalMove * Time.deltaTime);
        RotateTowards(moveDirection);
    }

    void ApplyGravity()
    {
        if (!allowMove)
        {
            velocity.x = 0f;
            velocity.z = 0f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    Vector3 GetMovementDirection()
    {
        if (cameraTransform == null)
        {
            Debug.LogError("cameraTransform is not assigned!");
            return Vector3.zero;
        }

        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        return right * moveInput.x + forward * moveInput.y;
    }

    void RotateTowards(Vector3 moveDirection)
    {
        if (!allowRotation) return;

        if (lockOnTarget != null)
        {
            Vector3 lookDirection = lockOnTarget.position - transform.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
            }
        }
        else if (moveDirection.sqrMagnitude > 0.1f)
        {
            Quaternion moveRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, moveRotation, 10f * Time.deltaTime);
        }
    }

    void UpdateAnimator(float inputMagnitude)
    {
        if (animator == null) return;

        float animSpeed = isRunning ? 1f : 0.5f;
        animator.SetFloat("Speed", inputMagnitude * animSpeed);
        animator.SetBool("IsRunning", isRunning && inputMagnitude > 0.1f);
        animator.SetBool("IsGrounded", isGrounded);

        if (IsCombat != previousIsCombat)
        {
            animator.SetBool("IsCombat", IsCombat);
            previousIsCombat = IsCombat;
        }

        Vector3 moveDirection = GetMovementDirection();
        if (IsCombat)
        {
            Vector3 localDir = transform.InverseTransformDirection(moveDirection);
            animator.SetFloat("Horizontal", localDir.x);
            animator.SetFloat("Vertical", localDir.z);
        }
        else
        {
            animator.SetFloat("Horizontal", 0f);
            animator.SetFloat("Vertical", 0f);
        }
    }

    // INPUT SYSTEM
    public void MoveInputHandler(InputAction.CallbackContext context)
    {
        if (isMovementLocked || !allowMove) return;

        moveInput = context.canceled ? Vector2.zero : context.ReadValue<Vector2>();
        UpdateAnimator(moveInput.magnitude);
    }

    public void RunInputHandler(InputAction.CallbackContext context)
    {
        if (isMovementLocked) return;

        isRunning = context.performed;
        IsPlayerRunning = isRunning;
        UpdateAnimator(moveInput.magnitude);
    }

    public void JumpInputHandler(InputAction.CallbackContext context)
    {
        if (isMovementLocked || !allowMove) return;

        if (context.performed && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger("jumpRun");
        }
    }

    public void LockOnInputHandler(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            lockOnTarget = (lockOnTarget != null) ? null : FindClosestTarget();
            Debug.Log($"LockOn: {lockOnTarget}");
        }
    }

    Transform FindClosestTarget()
    {
        float minDist = Mathf.Infinity;
        Transform closest = null;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = enemy.transform;
            }
        }
        return closest;
    }

    public void LockMovement(bool value)
    {
        isMovementLocked = value;
        if (value) StopMovement();
        Debug.Log($"LockMovement set to: {value}");
    }
}
