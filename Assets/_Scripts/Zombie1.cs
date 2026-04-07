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


    private void Patroling()
    {
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
    }

    private void ChasePlayer()
    {
        zombieAgent.SetDestination(LookPoint.position);
    }

    private void AttackPlayer()
    {
        zombieAgent.SetDestination(transform.position);
        transform.LookAt(LookPoint);
        
        if (attackTimer > 0){
        zombieAnim.SetTrigger("Attack");
        attackTimer = attackCooldown;
        LookPoint.GetComponent<HealthManager>().TakeDamage(15);
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
