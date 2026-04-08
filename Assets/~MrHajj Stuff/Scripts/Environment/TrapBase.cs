using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrapBase : MonoBehaviour
{
    [Header("Trap Base")]
    [SerializeField] [Range(1f, 12f)] private float trapTriggerDelay;
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            StartCoroutine(TrapDelay());
        }
    }

    private IEnumerator TrapDelay()
    {
        yield return new WaitForSeconds(trapTriggerDelay);
        TriggerTrap();
    }

    public virtual void TriggerTrap(){}
}
