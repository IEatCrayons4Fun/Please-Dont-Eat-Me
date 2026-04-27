using UnityEngine;
using System.Collections;

public interface IBulletPoolOwner
{
    void ReturnBulletToPool(GameObject bullet);
}

public class Bullet : MonoBehaviour
{
    [HideInInspector] public IBulletPoolOwner poolOwner;
    [HideInInspector] public float damage = 10f;
    [SerializeField] private float lifeSeconds = 5f;
    [SerializeField] private float damageCooldown = 0.5f;

    private float lifeTimer;
    private float lastDamageTime = -Mathf.Infinity;
    private Rigidbody rb;
    private Collider bulletCollider;
    private float spawnTime = -Mathf.Infinity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        bulletCollider = GetComponent<Collider>();
        // Do NOT set velocity here.
        // rb.velocity = ... // <- remove any velocity-setting here
    }

    private void OnEnable()
    {
        lifeTimer = lifeSeconds;
        if (bulletCollider != null)
            bulletCollider.enabled = false;
        // Do not initialize movement here. Movement happens in OnFired
    }

    public void OnFired(Vector3 direction, float speed)
    {
        spawnTime = Time.time;
        if (bulletCollider != null)
            bulletCollider.enabled = false;

        // make sure physics is enabled before setting velocity
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.linearVelocity = direction.normalized * speed;
        }
        lifeTimer = lifeSeconds;
        lastDamageTime = -Mathf.Infinity;
        StartCoroutine(EnableColliderAfterDelay());
    }

    private IEnumerator EnableColliderAfterDelay()
    {
        yield return new WaitForFixedUpdate();
        if (gameObject.activeInHierarchy && bulletCollider != null)
            bulletCollider.enabled = true;
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (poolOwner is TriggerTurret triggerTrap)
        {
            Transform trapRoot = triggerTrap.transform;
            if (other.transform == trapRoot || other.transform.IsChildOf(trapRoot))
            {
                return;
            }
        }

        if (Time.time - lastDamageTime < damageCooldown)
        {
            return;
        }

        bool hitEnemy = false;
        if (other.gameObject.CompareTag("Enemy"))
        {
            ZombieHP health = other.gameObject.GetComponent<ZombieHP>();
            if (health != null)
            {
                health.TakeDamage(damage);
                lastDamageTime = Time.time;
                hitEnemy = true;
                Debug.Log("Bullet hit " + other.gameObject.name + " for " + damage + " damage.");
            }
        }

        // Return to pool after hitting something meaningful.
        if (hitEnemy || !other.gameObject.CompareTag("Bullet"))
        {
            ReturnToPool();
        }
    }

    private void ReturnToPool()
    {
        if (poolOwner != null)
            poolOwner.ReturnBulletToPool(gameObject);
        else
            Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
            return;

        Gizmos.color = Color.cyan;

        if (col is SphereCollider sphere)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            Gizmos.matrix = oldMatrix;
            return;
        }

        if (col is BoxCollider box)
        {
            Matrix4x4 oldMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(box.center, box.size);
            Gizmos.matrix = oldMatrix;
            return;
        }

        Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
    }
}