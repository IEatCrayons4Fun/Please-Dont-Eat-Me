using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject checkpoint;
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    private Rigidbody rb;
    public Image fillBar;

    void Start()
    {
        currentHealth = maxHealth;
        rb = this.GetComponent<Rigidbody>();
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            Respawn();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
    }

    private void Respawn()
    {
        if (checkpoint != null)
        {
            this.transform.position = checkpoint.transform.position;
            rb.linearVelocity = Vector3.zero;
            currentHealth = maxHealth;
            UpdateHealthBar();
        }
    }

    private void UpdateHealthBar()
    {
        if (fillBar != null)
        {
            fillBar.fillAmount = currentHealth / maxHealth;
        }
    }
}