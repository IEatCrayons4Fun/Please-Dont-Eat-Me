using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            other.GetComponent<HealthManager>().checkpoint = this.gameObject;
        }
    }
}

