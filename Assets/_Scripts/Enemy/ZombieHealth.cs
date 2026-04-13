using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [Header("Zombie Health")]
    public float maxHealth = 100f;
    [SerializeField] public float currentHealth;

    private Animator zombieAnim;
    private NavMeshAgent zombieAgent;
    private Zombie1 zombie1;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        zombieAnim = GetComponent<Animator>();
        zombieAgent = GetComponent<NavMeshAgent>();
        zombie1 = GetComponent<Zombie1>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Play hurt animation
            if (zombieAnim != null)
                zombieAnim.SetTrigger("Hit");
        }
    }

    private void Die()
    {
        isDead = true;

        // Stop the zombie from moving and attacking
        zombieAgent.SetDestination(transform.position);
        zombieAgent.enabled = false;
        zombie1.enabled = false;

        // Play death animation
        if (zombieAnim != null)
            zombieAnim.SetTrigger("Death");

        // Destroy after death animation finishes
        Destroy(gameObject, 3f);
    }
}