using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Zombie1 : MonoBehaviour
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
    public bool playerInVisionRange;
    public bool playerInAttackingRange;

    [Header("Zombie Attacking")]
    public float attackCooldown = 1.5f;
    public float attackTimer = 0f;
    public Animator zombieAnim;

    public float damage = 0f;


    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
        zombieAnim = GetComponent<Animator>();
    }
    
    private void Update()
    {
        playerInVisionRange = Physics.CheckSphere(transform.position, visionRange, playerLayer);
        playerInAttackingRange = Physics.CheckSphere(transform.position, attackingRange, playerLayer);

        if (!playerInVisionRange && !playerInAttackingRange) Patroling();
        if (playerInVisionRange && !playerInAttackingRange) ChasePlayer();
        if (playerInAttackingRange && playerInVisionRange) AttackPlayer();

        if (attackTimer > 0f){
            attackTimer -= Time.deltaTime;
        }
            
    }
    private void SetAnimationState(string state)
    {
        zombieAnim.SetBool("IsWalking", state == "IsWalking");
        zombieAnim.SetBool("IsIdle", state == "IsIdle");
    }
    

    private void Patroling()
    {
        if(walkPoints.Length == 0) return; // No walk points assigned, skip patrolling

        if(Vector3.Distance(walkPoints[currentZombiePosition].transform.position, transform.position) < walkingPointRadius)
        {
            currentZombiePosition = Random.Range(0, walkPoints.Length);
            if(currentZombiePosition >= walkPoints.Length)
            {
                currentZombiePosition = 0;
            }
        }
        zombieAgent.SetDestination(walkPoints[currentZombiePosition].transform.position);
        //changes zombie facing
        SetAnimationState("IsWalking");
    }

    private void ChasePlayer()
    {
        zombieAgent.SetDestination(LookPoint.position);
        SetAnimationState("IsWalking");

    }

    private void AttackPlayer()
    {
        zombieAgent.SetDestination(transform.position);
        transform.LookAt(LookPoint);
        SetAnimationState("IsIdle");
        if (attackTimer <= 0){
            if (zombieAnim != null){
                zombieAnim.SetTrigger("Attack");
            }
            attackTimer = attackCooldown;
            if (Player != null){
                Player.TakeDamage(damage);
            }
            else{
                Debug.LogWarning("Player does not have a HealthManager component.");
            }   
        }
        
        
    }


    //Checking Aggro Ranges
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackingRange);
    }
}
