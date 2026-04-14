using UnityEngine;
using UnityEngine.AI;

public class ZombieHP : MonoBehaviour
{
    [Header("NPC Health")]
    public float maxHealth = 100f;
    [SerializeField] public float currentHealth;

    private Animator zombieAnim;
    private NavMeshAgent zombieAgent;
    private Zombie zombie;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        zombieAnim = GetComponent<Animator>();
        zombieAgent = GetComponent<NavMeshAgent>();
        zombie = GetComponent<Zombie>();
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
        zombie.enabled = false;

        // Play death animation
        if (zombieAnim != null)
            zombieAnim.SetTrigger("Death");
        LootDropper dropper = GetComponent<LootDropper>();
        if (dropper != null) dropper.DropLoot();
        // Destroy after death animation finishes
        Destroy(gameObject, 3f);
    }
}