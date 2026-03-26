using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public TriggerTurret parentTrap;
    public float damage;
    public Rigidbody rb;
    public float maxLifetime;
    private Coroutine lifetime;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        Debug.Log("Bullet enabled, maxLifetime: " + maxLifetime);
        if (lifetime == null)
        {
            lifetime = StartCoroutine(MaxLifetime());
        }
        else
        {
            StopCoroutine(lifetime);
            lifetime = StartCoroutine(MaxLifetime());
        }
    }

    private void OnDisable()
    {
        // Clean up coroutine when disabled so it doesnt run in background
        if (lifetime != null)
        {
            StopCoroutine(lifetime);
            lifetime = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.CompareTag("TurretTrap"))
        {
            if (other.gameObject.CompareTag("Enemy"))
            {
                HealthManager hm = other.GetComponent<HealthManager>();
                if (hm != null) hm.TakeDamage(damage);
            }
            this.gameObject.SetActive(false);
            if (parentTrap != null)
            {
                parentTrap.bullets.Add(this.gameObject);
            }
            else
            {
                Debug.LogWarning("Bullet has no parentTrap assigned!");
            }
            rb.linearVelocity = Vector3.zero;
        }
    }

    private IEnumerator MaxLifetime()
    {
        yield return new WaitForSeconds(maxLifetime);
        this.gameObject.SetActive(false);
        if (parentTrap != null)
        {
            parentTrap.bullets.Add(this.gameObject);
        }
        else
        {
            Debug.LogWarning("Bullet has no parentTrap assigned!");
        }
        rb.linearVelocity = Vector3.zero;
    }
}