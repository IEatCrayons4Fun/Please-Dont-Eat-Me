using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    [Range(1f, 25f)]
    public float walkSpeed;

    [Range(1f, 50f)]
    public float sprintSpeed;

    [Range(1f, 10f)]
    public float jumpHeight = 2f;

    private InputAction jump;
    private InputAction sprint;
    private bool isSprinting;

    public bool grounded;
    [SerializeField] private LayerMask GroundLayer;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;     // assign a child empty at the feet
    [SerializeField] private float groundCheckRadius = 0.2f;

    private Vector3 horizontalVelocity;
    float speed;

    [SerializeField]
    [Range(1f, 130f)]
    private float baseFOV = 60f;

    [SerializeField]
    [Range(1f, 130f)]
    private float sprintFOV = 90f;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    [Range(1f, 10f)]
    private float fovTransitionTime;

    private Coroutine changingFOV;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = InputSystem.actions.FindAction("Jump");
        sprint = InputSystem.actions.FindAction("Sprint");
    }

    private void FixedUpdate()
    {
        // Use a LayerMask-based sphere check to set grounded
        grounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, GroundLayer);

        rb.linearDamping = grounded ? 6f : 0f;
        MovePlayer();
        DetectJump();
    }

    private void Update()
    {
        DetectSprint();
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // removed OnCollisionEnter/Stay/Exit tag checks — replaced by LayerMask-based ground check

    private void MovePlayer()
    {
        speed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 inputDir = (transform.forward * movementY + transform.right * movementX).normalized;
        Vector3 targetVelocity = inputDir * speed;

        if (grounded)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.x = Mathf.Lerp(velocity.x, targetVelocity.x, Time.fixedDeltaTime * 10f);
            velocity.z = Mathf.Lerp(velocity.z, targetVelocity.z, Time.fixedDeltaTime * 10f);
            rb.linearVelocity = velocity;
        }
        else
        {
            rb.AddForce(targetVelocity * 0.3f, ForceMode.Acceleration);
        }
    }

    private void DetectJump()
    {
        if (jump.triggered && grounded)
        {
            grounded = false;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        }
    }

    private void DetectSprint()
    {
        if (sprint.WasPressedThisFrame())
        {
            isSprinting = true;
            if (changingFOV == null)
            {
                changingFOV = StartCoroutine(LerpCamFOV(sprintFOV, fovTransitionTime));
            }
            else
            {
                StopCoroutine(changingFOV);
                changingFOV = StartCoroutine(LerpCamFOV(sprintFOV, fovTransitionTime));
            }
        }
        else if (sprint.WasReleasedThisFrame())
        {
            isSprinting = false;
            if (changingFOV == null)
            {
                changingFOV = StartCoroutine(LerpCamFOV(baseFOV, fovTransitionTime));
            }
            else
            {
                StopCoroutine(changingFOV);
                changingFOV = StartCoroutine(LerpCamFOV(baseFOV, fovTransitionTime));
            }
        }
    }

    private IEnumerator LerpCamFOV(float newFOV, float transitionTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < transitionTime)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newFOV, transitionTime * Time.deltaTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    // Visualize the ground check in the editor
    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}