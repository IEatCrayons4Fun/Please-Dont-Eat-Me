using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public TriggerTurret parentTrap;
    [HideInInspector] public float damage = 10f;
    [SerializeField] private float lifeSeconds = 5f;
    [SerializeField] private float damageCooldown = 0.5f;

    private float lifeTimer;
    private float lastDamageTime = -Mathf.Infinity;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // Do NOT set velocity here.
        // rb.velocity = ... // <- remove any velocity-setting here
    }

    private void OnEnable()
    {
        lifeTimer = lifeSeconds;
        // Do not initialize movement here. Movement happens in OnFired
    }

    public void OnFired(Vector3 direction, float speed)
    {
        // make sure physics is enabled before setting velocity
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = false;
            rb.linearVelocity = direction.normalized * speed;
        }
        lifeTimer = lifeSeconds;
        lastDamageTime = -Mathf.Infinity;
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f) ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (parentTrap != null)
        {
            Transform trapRoot = parentTrap.transform;
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
            ZombieHealth health = other.gameObject.GetComponent<ZombieHealth>();
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
        if (parentTrap != null) parentTrap.ReturnBulletToPool(gameObject);
        else Destroy(gameObject);
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