using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public GameObject checkpoint;
    public int maxHealth = 100;
    [SerializeField] private float currentHealth;
    private Rigidbody rb;

    private void Start(){
        currentHealth = maxHealth;
        rb = this.GetComponent<Rigidbody>();
    }

    public void TakeDamage(float damage){
        currentHealth -= damage;
        //Debug.Log("Took Damage: " +damage);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        if(currentHealth <= 0){
            //You might want to change this if you wanted a respawn screen.
            Respawn();
        }
    }

    public void Heal(float healAmount){
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    private void Respawn(){
        Debug.Log("You Respawned");
        if(checkpoint != null){
            this.transform.position = checkpoint.transform.position;
            rb.linearVelocity = Vector3.zero;
        }
    }
}
