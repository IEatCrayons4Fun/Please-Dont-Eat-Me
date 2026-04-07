using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HealthManager healthManager = other.GetComponent<HealthManager>();
            if (healthManager != null)
            {
                healthManager.checkpoint = this.gameObject;
            }
        }
    }
}

