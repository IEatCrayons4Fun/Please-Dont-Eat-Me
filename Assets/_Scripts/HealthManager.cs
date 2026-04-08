using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject checkpoint;
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    private Rigidbody rb;

    public Image fillBar;

    void Start(){
        currentHealth = maxHealth;
        rb = this.GetComponent<Rigidbody>();
        UpdateHealthBar();
    }

    public void TakeDamage(float damage){
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Took Damage: " + damage + " | HP Remaining: " + currentHealth);
        UpdateHealthBar();
        if(currentHealth <= 0){
            //You might want to change this if you wanted a respawn screen.
            Respawn();
        }
    }

    public void Heal(float healAmount){
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void Respawn(){
        Debug.Log("You Respawned");
        if(checkpoint != null){
            this.transform.position = checkpoint.transform.position;
            rb.linearVelocity = Vector3.zero;
            UpdateHealthBar();
        }
    }
    private void UpdateHealthBar(){
        if(fillBar != null){
            fillBar.fillAmount = currentHealth / maxHealth;
        }
    }

}
