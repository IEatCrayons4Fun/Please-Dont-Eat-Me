using UnityEngine;

public class NPCHealth : MonoBehaviour
{
    [Header("NPC Health")]
    public float maxHealth = 100f;
    [SerializeField] public float currentHealth;
    public bool isDamagedable = true;
    public bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage)
    {
        if(!isDamagedable) return;
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            currentHealth = maxHealth; 
        }
    }
}
