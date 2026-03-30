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

    private void Awake()
    {
        zombieAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        playerInVisionRange = Physics.CheckSphere(transform.position, visionRange, playerLayer);
        playerInAttackingRange = Physics.CheckSphere(transform.position, attackingRange, playerLayer);

        if (!playerInVisionRange && !playerInAttackingRange) Patroling();
        if (playerInVisionRange && !playerInAttackingRange) ChasePlayer();
        if (playerInAttackingRange && playerInVisionRange) AttackPlayer();
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
        transform.position = Vector3.MoveTowards(transform.position, walkPoints[currentZombiePosition].transform.position, zombieSpeed * Time.deltaTime);
        //changes zombie facing
    }

    private void ChasePlayer()
    {
        //
    }

    private void AttackPlayer()
    {
        //
    }


    //Checking Aggro Ranges
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackingRange);
    }
}
