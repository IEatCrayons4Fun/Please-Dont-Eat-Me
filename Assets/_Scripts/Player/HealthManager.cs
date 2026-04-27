using UnityEngine;
using UnityEngine.UI;

public class HealthManager : MonoBehaviour
{
    public GameObject checkpoint;
    public float maxHealth = 100f;
    [HideInInspector] public float currentHealth;
    private Rigidbody rb;
    public Image fillBar;

    

    [Header("Shield")]
    public Image shieldBar;
    [HideInInspector] public float currentShield = 0f;
    [HideInInspector] public float maxShield = 100f;
    public bool hasShield;

    public bool IsTesting;

    void Start()
    {
        currentHealth = maxHealth;
        rb = this.GetComponent<Rigidbody>();
        UpdateHealthBar();
        if (shieldBar != null)
        {
            shieldBar.gameObject.SetActive(false);
        }
    }

    

    public void TakeDamage(float damage)
    {
        damage = AbsorbDamage(damage);
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        if (currentHealth <= 0)
        {
            if (IsTesting)
            {
                currentHealth = maxHealth;
                UpdateHealthBar();
            }
            Respawn();
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthBar();
        Debug.Log("Healed");
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

    private void UpdateShieldBar()
    {
        if (shieldBar != null)
        {
            shieldBar.gameObject.SetActive(hasShield);
            shieldBar.fillAmount = currentShield / maxShield;
        }
    }

    public void AddShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0, maxShield);
        Debug.Log("Shield: " + currentShield);
        hasShield = true;
        UpdateShieldBar();
    }

    public float AbsorbDamage(float damage)
    {
        if (hasShield)
        {
            float damageAbsorbed = Mathf.Min(currentShield, damage);
            currentShield -= damageAbsorbed;
            UpdateShieldBar();
            if (currentShield <= 0)
            {
                hasShield = false;
            }
            return damage - damageAbsorbed; // Return remaining damage to be applied to health
        }
        return damage; // No shield, full damage applies to health
    }

}