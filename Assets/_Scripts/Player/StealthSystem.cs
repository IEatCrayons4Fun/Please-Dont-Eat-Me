using UnityEngine;
using UnityEngine.InputSystem;

public class StealthSystem : MonoBehaviour
{
    [Header("Stealth Settings")]
    [SerializeField] private float stealthFOVThreshold = 45f; // Player FOV when moving slowly
    [SerializeField] private float detectionRange = 20f;
    [SerializeField] private float detectionAngle = 120f;
    [SerializeField] private float baseNoiseLevel = 0f;
    [SerializeField] private float sprintNoiseLevel = 1f;
    [SerializeField] private float crouchWalkSpeed = 3f;
    
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private float currentNoiseLevel;
    private bool isCrouching;
    private InputAction crouch;
    
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        crouch = InputSystem.actions.FindAction("Crouch"); // Add this to your input map
    }
    
    private void Update()
    {
        HandleCrouch();
        CalculateNoiseLevel();
    }
    
    private void HandleCrouch()
    {
        if (crouch.WasPressedThisFrame())
        {
            isCrouching = !isCrouching;
            
            // Lower/raise camera for visual feedback
            if (isCrouching)
            {
                transform.position += Vector3.down * 0.5f;
                playerMovement.walkSpeed = crouchWalkSpeed;
            }
            else
            {
                transform.position += Vector3.up * 0.5f;
                playerMovement.walkSpeed = 8f; // Reset to default
            }
        }
    }
    
    private void CalculateNoiseLevel()
    {
        // Crouching = quieter, sprinting = louder
        if (isCrouching)
        {
            currentNoiseLevel = baseNoiseLevel;
        }
        else if (playerMovement != null && rb.linearVelocity.magnitude > playerMovement.walkSpeed * 0.8f)
        {
            currentNoiseLevel = sprintNoiseLevel;
        }
        else
        {
            currentNoiseLevel = baseNoiseLevel * 0.5f;
        }
    }
    
    public float GetNoiseLevel()
    {
        return currentNoiseLevel;
    }
    
    public bool IsCrouching()
    {
        return isCrouching;
    }
}