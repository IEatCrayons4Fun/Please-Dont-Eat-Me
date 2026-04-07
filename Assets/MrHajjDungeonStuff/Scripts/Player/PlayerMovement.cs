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

    [Range(1f, 20f)]
    public float jumpHeight = 7f;

    private InputAction jump;
    private InputAction sprint;
    private bool isSprinting;

    [HideInInspector]
    public bool grounded;

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

    public LayerMask groundLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        jump = InputSystem.actions.FindAction("Jump");
        sprint = InputSystem.actions.FindAction("Sprint");
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0)
            grounded = true;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0)
            grounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & groundLayer.value) != 0)
            grounded = false;
    }

    private void MovePlayer()
    {
        if (rb == null) return;

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
        if (jump == null || !jump.triggered || !grounded || rb == null) return;

        grounded = false;
        var velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        rb.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
    }

    private void DetectSprint()
    {
        if (sprint == null) return;

        if (sprint.WasPressedThisFrame())
        {
            isSprinting = true;
            if (changingFOV != null) StopCoroutine(changingFOV);
            changingFOV = StartCoroutine(LerpCamFOV(sprintFOV, fovTransitionTime));
        }
        else if (sprint.WasReleasedThisFrame())
        {
            isSprinting = false;
            if (changingFOV != null) StopCoroutine(changingFOV);
            changingFOV = StartCoroutine(LerpCamFOV(baseFOV, fovTransitionTime));
        }
    }

    private IEnumerator LerpCamFOV(float newFOV, float transitionTime)
    {
        float elapsedTime = 0f;
        float startFOV = cam.fieldOfView;
        while (elapsedTime < transitionTime)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV, newFOV, elapsedTime / transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = newFOV;
    }
}