using UnityEngine;

public class PitScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other){
        if(other.gameObject.CompareTag("Player")){
            HealthManager player = other.GetComponent<HealthManager>();
            player.TakeDamage(player.maxHealth);
        }
    }
}
