using Photon.Pun;
using UnityEngine;

public class PlayerMovement : MonoBehaviourPun
{
    [Header("Main")]
    public PlayerSO playerStats;
    [HideInInspector] private Rigidbody rb;
    [SerializeField] public Animator animator;

    [Header("Movement")]
    [HideInInspector] private Vector2 moveInput;
    [SerializeField] private Vector3 currentVelocity = Vector3.zero;
    [SerializeField] private Vector3 currentSpeed = Vector3.zero;
    [HideInInspector] private Vector3 animationSpeed = Vector3.zero;
    [SerializeField] private bool isSprinting = false;

    [Header("Jump")]
    [HideInInspector] private bool isGrounded = false;
    [HideInInspector] private bool isFalling = false;

    void Start()
    {
        rb = GetComponentInParent<Rigidbody>();
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (transform.position.y < -1f)
        {
            transform.position = new Vector3(transform.position.x, 3f, transform.position.z);
            rb.linearVelocity = Vector3.zero;
        }
        CheckGround();


        animationSpeed.z = Mathf.Lerp(animationSpeed.z, currentVelocity.z, playerStats.acceleration * Time.fixedDeltaTime);
        if (Mathf.Abs(animationSpeed.z) < 0.1f) animationSpeed.z = 0f;
        animator.SetFloat("Vertical", animationSpeed.z);

        animationSpeed.x = Mathf.Lerp(animationSpeed.x, currentVelocity.x, playerStats.acceleration * Time.fixedDeltaTime);
        if (Mathf.Abs(animationSpeed.x) < 0.1f) animationSpeed.x = 0f;
        animator.SetFloat("Horizontal", animationSpeed.x);

        animator.SetBool("IsGrounded", isGrounded);
        animator.SetBool("IsFalling", isFalling);
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    public void ProcessMovement(Vector2 input)
    {
        moveInput = input;
    }

    private void MovePlayer()
    {

        Vector3 inputDir = new Vector3(moveInput.x, 0, moveInput.y).normalized;

        // Choose max speed based on sprinting and forward movement
        Vector3 targetSpeed = playerStats.maxWalkSpeed;
        if (isSprinting && inputDir.z > 0)
        {
            targetSpeed = playerStats.maxSprintSpeed;
        }

        // Smooth acceleration/deceleration
        if (inputDir.magnitude > 0.1f)
        {
            currentSpeed.x = Mathf.Lerp(currentSpeed.x, targetSpeed.x, playerStats.acceleration * Time.fixedDeltaTime);
            currentSpeed.z = Mathf.Lerp(currentSpeed.z, targetSpeed.z, playerStats.acceleration * Time.fixedDeltaTime);
        }
        else
        {
            currentSpeed.x = Mathf.Lerp(currentSpeed.x, 0f, playerStats.acceleration * Time.fixedDeltaTime);
            currentSpeed.z = Mathf.Lerp(currentSpeed.z, 0f, playerStats.acceleration * Time.fixedDeltaTime);
        }

        currentVelocity = new Vector3(inputDir.x * currentSpeed.x, 0f, inputDir.z * currentSpeed.z);

        Vector3 worldMove = transform.TransformDirection(currentVelocity);
        rb.linearVelocity = new Vector3(worldMove.x, rb.linearVelocity.y, worldMove.z);
    }


    public void SprintStart()
    {
        isSprinting = true;
    }

    public void SprintEnd()
    {
        isSprinting = false;
    }

    public void CheckGround()
    {
        // Check is ground by raycasting
        Vector3 position = transform.position + new Vector3(0, 0.2f, 0);
        isGrounded = Physics.Raycast(position, Vector3.down, 0.5f, playerStats.groundLayer);
        Debug.DrawRay(position, Vector3.down * (0.2f), isGrounded ? Color.green : Color.red);

        if (rb.linearVelocity.y < 0 && Physics.Raycast(transform.position, Vector3.down, 0.6f, playerStats.groundLayer))
        {
            isFalling = true;
        }
        else
        {
            isFalling = false;
        }
    }

    public void Jump()
    {
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * playerStats.jumpForce, ForceMode.Impulse);
        }
    } 
}
