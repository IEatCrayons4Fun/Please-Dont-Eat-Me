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
    public float detectionSensitivity = 1f;
    public float sightAlertRate = 1f;
    public float detectionAngle = 120f;
    public float alertDecayRate = 0.5f;
    private StealthSystem playerStealth;
    private float alertness = 0f;

    [Header("Zombie Attacking")]
    public float attackCooldown = 1.5f;
    public float attackTimer = 0f;
    public Animator zombieAnim;
    public float damage = 0f;

    private enum ZombieState { Patrolling, Investigating, Chasing, Attacking }
    private ZombieState currentState = ZombieState.Patrolling;

    private Coroutine slowCoroutine = null;
    private GameObject player;

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        zombieAnim = GetComponent<Animator>();
        if (zombieAgent != null)
            zombieAgent.stoppingDistance = attackingRange;
    }

    private void Start()
    {
        player = PlayerSingleton.instance.gameObject;
        playerStealth = player.GetComponent<StealthSystem>();
    }

    private void Update()
    {
        playerInAttackingRange = Physics.CheckSphere(transform.position + sphereOffset, attackingRange, playerLayer);
        playerInVisionRange = IsPlayerVisible();

        if (playerStealth != null)
            DetectBySound();

        // Build alertness from sight
        if (playerInVisionRange)
            alertness = Mathf.Min(alertness + sightAlertRate * Time.deltaTime, 1f);
        else
            alertness -= alertDecayRate * Time.deltaTime;

        alertness = Mathf.Clamp01(alertness);

        // State machine
        if (playerInAttackingRange && (playerInVisionRange || currentState == ZombieState.Chasing))
            currentState = ZombieState.Attacking;
        else if (alertness >= 1f)
            currentState = ZombieState.Chasing;
        else if (playerInVisionRange && alertness < 1f)
            currentState = ZombieState.Investigating;
        else if (!playerInVisionRange && alertness <= 0f)
            currentState = ZombieState.Patrolling;

        switch (currentState)
        {
            case ZombieState.Patrolling:    Patroling(); break;
            case ZombieState.Investigating: Investigate(); break;
            case ZombieState.Chasing:       ChasePlayer(); break;
            case ZombieState.Attacking:     AttackPlayer(); break;
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    private bool IsPlayerVisible()
    {
        if (!Physics.CheckSphere(transform.position, visionRange, playerLayer))
            return false;

        Vector3 directionToPlayer = LookPoint.position - transform.position;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > detectionAngle * 0.5f)
            return false;

        if (playerStealth != null && playerStealth.IsCrouching())
        {
            if (directionToPlayer.magnitude > visionRange * 0.5f)
                return false;
        }

        // Line of sight — blocked by walls
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, visionRange))
        {
            if (hit.transform != LookPoint.transform.root)
                return false;
        }

        return true;
    }

    private void DetectBySound()
    {
        Vector3 directionToPlayer = LookPoint.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        float noiseLevel = playerStealth.GetNoiseLevel();

        // Walls muffle sound
        if (Physics.Raycast(transform.position, directionToPlayer.normalized, out RaycastHit hit, visionRange))
        {
            if (hit.transform != LookPoint.transform.root && hit.distance < distanceToPlayer)
                noiseLevel *= 0.2f;
        }

        float detectionThreshold = visionRange * (noiseLevel * detectionSensitivity);

        if (distanceToPlayer < detectionThreshold && noiseLevel > 0)
        {
            float angle = Vector3.Angle(transform.forward, directionToPlayer);
            bool inCone = angle <= detectionAngle * 0.5f;

            float buildRate = inCone ? detectionSensitivity * 2f : detectionSensitivity * 1f;
            alertness = Mathf.Min(alertness + (noiseLevel * buildRate) * Time.deltaTime, 1f);
        }
    }

    private void Investigate()
    {
        zombieAgent.isStopped = true;
        transform.LookAt(new Vector3(LookPoint.position.x, transform.position.y, LookPoint.position.z));
        SetAnimationState("IsIdle");
    }

    private void SetAnimationState(string state)
    {
        zombieAnim.SetBool("IsWalking", state == "IsWalking");
        zombieAnim.SetBool("IsIdle", state == "IsIdle");
    }

    private void Patroling()
    {
        zombieAgent.isStopped = false;

        if (walkPoints.Length == 0)
        {
            zombieAgent.isStopped = true;
            SetAnimationState("IsIdle");
            return;
        }

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

    public float GetAlertness()
    {
        return alertness;
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
        // Vision range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + sphereOffset, attackingRange);

        // Detection cone
        Gizmos.color = Color.blue;
        Vector3 forward = transform.forward * visionRange;
        Vector3 leftBound = Quaternion.Euler(0, -detectionAngle * 0.5f, 0) * forward;
        Vector3 rightBound = Quaternion.Euler(0, detectionAngle * 0.5f, 0) * forward;
        Gizmos.DrawRay(transform.position, leftBound);
        Gizmos.DrawRay(transform.position, rightBound);
        Gizmos.DrawRay(transform.position, forward);


        if (!Application.isPlaying) return;

        // Line of sight ray to player
        if (LookPoint != null)
        {
            Gizmos.color = playerInVisionRange ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, LookPoint.position);
            Gizmos.DrawWireSphere(LookPoint.position, 0.3f);
        }

        // Alertness bar above zombie head
        Vector3 barBase = transform.position + Vector3.up * 2.5f;
        float barHeight = 1.5f;

        Gizmos.color = Color.grey;
        Gizmos.DrawLine(barBase, barBase + Vector3.up * barHeight);

        if (alertness > 0f)
        {
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, alertness);
            Gizmos.DrawLine(barBase, barBase + Vector3.up * (barHeight * alertness));
        }

        // State indicator sphere
        Vector3 statePos = transform.position + Vector3.up * 2.2f;
        switch (currentState)
        {
            case ZombieState.Patrolling:    Gizmos.color = Color.white; break;
            case ZombieState.Investigating: Gizmos.color = Color.yellow; break;
            case ZombieState.Chasing:       Gizmos.color = Color.red; break;
            case ZombieState.Attacking:     Gizmos.color = new Color(1f, 0f, 0f, 1f); break;
        }
        Gizmos.DrawSphere(statePos, 0.2f);

        // Sound detection threshold ring
        if (playerStealth != null)
        {
            float noiseLevel = playerStealth.GetNoiseLevel();
            float detectionThreshold = visionRange * (noiseLevel * detectionSensitivity);
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawWireSphere(transform.position, detectionThreshold);
        }

        // Wall obstruction hit point
        if (LookPoint != null)
        {
            Vector3 dir = (LookPoint.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, dir, out RaycastHit hit, visionRange))
            {
                if (hit.transform != LookPoint.transform.root)
                {
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawWireSphere(hit.point, 0.2f);
                    Gizmos.DrawLine(transform.position, hit.point);
                }
            }
        }
    }
}