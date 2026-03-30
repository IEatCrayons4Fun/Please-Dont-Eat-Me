using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
    public TriggerTurret parentTrap;
    public float damage;
    public Rigidbody rb;
    public float maxLifetime;
    private Coroutine lifetime;
    private Vector3 direction;
    private float speed;

    private void Start()
    {
        rb = this.GetComponent<Rigidbody>();
    }

    public void Fire(Vector3 fireDirection, float fireSpeed)
    {
        direction = fireDirection.normalized;
        speed = fireSpeed;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    private void OnEnable()
    {
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
        if (lifetime != null)
        {
            StopCoroutine(lifetime);
            lifetime = null;
        }
        direction = Vector3.zero;
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
                this.transform.SetParent(parentTrap.transform);
                parentTrap.bullets.Add(this.gameObject);
            }
        }
    }

    private IEnumerator MaxLifetime()
    {
        yield return new WaitForSeconds(maxLifetime);
        this.gameObject.SetActive(false);
        if (parentTrap != null)
        {
            this.transform.SetParent(parentTrap.transform);
            parentTrap.bullets.Add(this.gameObject);
        }
    }
}