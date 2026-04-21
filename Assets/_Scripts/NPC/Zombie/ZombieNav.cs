using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    [Header("Zombie Stuff")]
    public NavMeshAgent zombieAgent;
    public Transform LookPoint;
    public LayerMask playerLayer;
    public HealthManager Player;

    [Header("Zombie Guarding Points")]
    public GameObject[] walkPoints;
    int currentZombiePosition = 0;
    public float zombieSpeed;
    float walkingPointRadius = 2;

    [Header("Zombie Aggro")]
    public float visionRange;
    public float attackingRange;
    public Vector3 sphereOffset;
    public bool playerInVisionRange;
    public bool playerInAttackingRange;
    
    [Header("Stealth Detection")]
    public float detectionSensitivity = 1f; // 0-1, lower = easier to detect stealth
    private StealthSystem playerStealth;
    private float alertness = 0f; // 0-1 scale
    public float alertDecayRate = 0.5f; // How fast alertness decreases

    [Header("Zombie Attacking")]
    public float attackCooldown = 1.5f;
    public float attackTimer = 0f;
    public Animator zombieAnim;
    public float damage = 0f;

    private Coroutine slowCoroutine = null;

    private GameObject player;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        zombieAnim = GetComponent<Animator>();
        if (zombieAgent == null)
        {
            zombieAgent.stoppingDistance = attackingRange;
        }
        
        // Find the player's stealth system
        player = PlayerSingleton.instance.gameObject;
        StealthSystem player = GetComponent<StealthSystem>();

    }

    private void Update()
    {
        // Standard detection
        playerInVisionRange = Physics.CheckSphere(transform.position, visionRange, playerLayer);
        playerInAttackingRange = Physics.CheckSphere(transform.position + sphereOffset, attackingRange, playerLayer);

        // Add stealth detection
        if (playerStealth != null)
        {
            DetectBySound();
        }

        if (!playerInVisionRange && !playerInAttackingRange && alertness <= 0) 
            Patroling();
        if ((playerInVisionRange || alertness > 0.5f) && !playerInAttackingRange) 
            ChasePlayer();
        if (playerInAttackingRange && playerInVisionRange) 
            AttackPlayer();

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
        
        // Decay alertness over time
        if (alertness > 0f)
            alertness -= alertDecayRate * Time.deltaTime;
    }

    private void DetectBySound()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, LookPoint.position);
        float noiseLevel = playerStealth.GetNoiseLevel();
        
        // Sound detection: closer + louder = easier to detect
        float detectionThreshold = visionRange * (1 - (noiseLevel * detectionSensitivity));
        
        if (distanceToPlayer < detectionThreshold && noiseLevel > 0)
        {
            alertness = Mathf.Min(alertness + (noiseLevel * 0.5f), 1f);
        }
    }

    private void SetAnimationState(string state)
    {
        zombieAnim.SetBool("IsWalking", state == "IsWalking");
        zombieAnim.SetBool("IsIdle", state == "IsIdle");
    }

    private void Patroling()
    {
        if (walkPoints.Length == 0) return;

        if (Vector3.Distance(walkPoints[currentZombiePosition].transform.position, transform.position) < walkingPointRadius)
        {
            currentZombiePosition = Random.Range(0, walkPoints.Length);
            if (currentZombiePosition >= walkPoints.Length)
                currentZombiePosition = 0;
        }

        zombieAgent.SetDestination(walkPoints[currentZombiePosition].transform.position);
        SetAnimationState("IsWalking");
    }

    private void ChasePlayer()
    {
        zombieAgent.isStopped = false;
        zombieAgent.SetDestination(LookPoint.position);
        SetAnimationState("IsWalking");
    }

    private void AttackPlayer()
    {
        zombieAgent.isStopped = true;
        transform.LookAt(LookPoint);
        SetAnimationState("IsIdle");

        if (attackTimer <= 0)
        {
            if (zombieAnim != null)
                zombieAnim.SetTrigger("Attack");

            attackTimer = attackCooldown;

            if (Player != null)
                Player.TakeDamage(damage);
            else
                Debug.LogWarning("Player does not have a HealthManager component.");
        }
    }

    public void ApplySlowIndefinite(float multiplier)
    {
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
            slowCoroutine = null;
        }
        zombieAgent.speed = zombieSpeed * multiplier;
    }

    public void StartSlowCountdown(float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);

        slowCoroutine = StartCoroutine(SlowCoroutine(duration));
    }

    private IEnumerator SlowCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        zombieAgent.speed = zombieSpeed;
        slowCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, attackingRange);
    }
}