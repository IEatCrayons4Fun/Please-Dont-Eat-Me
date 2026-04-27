using UnityEngine;
using UnityEngine.InputSystem;

public class StealthSystem : MonoBehaviour
{
    [Header("Stealth Settings")]
    [SerializeField] private float baseNoiseLevel = 0f;
    [SerializeField] private float sprintNoiseLevel = 1f;
    [SerializeField] private float crouchWalkSpeed = 3f;
    
    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private float currentNoiseLevel;
    private bool isCrouching;
    private InputAction crouch;

    [SerializeField] private Transform Camera;
    
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
                Camera.localPosition += Vector3.down * 0.5f;
                playerMovement.walkSpeed = crouchWalkSpeed;
            }
            else
            {
                Camera.localPosition += Vector3.up * 0.5f;
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