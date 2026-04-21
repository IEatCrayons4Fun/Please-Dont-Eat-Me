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
        zombieAgent.SetDestination(transform.position);
        zombieAgent.enabled = false;
        zombie.enabled = false;

        if (zombieAnim != null)
            zombieAnim.SetTrigger("Death");

        // Switch to dead layer so player can walk through
        gameObject.layer = LayerMask.NameToLayer("Dead Zombie");

        LootDropper dropper = GetComponent<LootDropper>();
        if (dropper != null) dropper.DropLoot();

        Destroy(gameObject, 2.5f);
    }
}