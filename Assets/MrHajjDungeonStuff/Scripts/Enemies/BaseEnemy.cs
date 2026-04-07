using UnityEngine;
using UnityEngine.AI;

public class BaseEnemy : MonoBehaviour
{

    public GameObject player;
    private NavMeshAgent navMeshAgent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        player = PlayerSingleton.instance.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(player != null){
            navMeshAgent.SetDestination(player.transform.position);
        }
    }
}