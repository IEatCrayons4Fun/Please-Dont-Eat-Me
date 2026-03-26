using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerTurret : MonoBehaviour
{
    [Header("Turret Trap")]
    [SerializeField] [Range(0.1f, 10f)] private float fireRate;
    [SerializeField] [Range(1f, 100f)] private float damage;
    [SerializeField] [Range(1f, 100f)] private int trapDuration;
    [SerializeField] GameObject[] firePoints;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] private int maxBulletCount;
    [SerializeField] [Range(1f, 100f)] private float bulletSpeed;
    [HideInInspector] public List<GameObject> bullets = new List<GameObject>();
    public bool canFire = true;
    public List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isFiring = false;

    private void Start()
    {
        Debug.Log("TriggerTurret Start - maxBulletCount: " + maxBulletCount);

        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            Debug.LogError("NO COLLIDER FOUND ON " + gameObject.name + "! Add a Sphere Collider.");
        }
        else if (!col.isTrigger)
        {
            Debug.LogError("Collider on " + gameObject.name + " is NOT set to trigger!");
        }
        else
        {
            Debug.Log("Collider OK - isTrigger: " + col.isTrigger);
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogError("NO FIRE POINTS ASSIGNED on " + gameObject.name);
        }
        else
        {
            Debug.Log("Fire points count: " + firePoints.Length);
        }

        if (bulletPrefab == null)
        {
            Debug.LogError("NO BULLET PREFAB ASSIGNED on " + gameObject.name);
        }
        else
        {
            Debug.Log("Bullet prefab OK: " + bulletPrefab.name);
        }

        StartCoroutine(SpawnBullets());
    }

    private IEnumerator SpawnBullets()
    {
        bullets.Clear();
        while (bullets.Count < maxBulletCount)
        {
            GameObject bullet = Instantiate(bulletPrefab, this.transform);
            bullets.Add(bullet);
            bullet.SetActive(false);
        }
        Debug.Log("Bullet pool ready - total bullets: " + bullets.Count);
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter hit: " + other.gameObject.name + " | tag: " + other.gameObject.tag);
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Enemy detected: " + other.gameObject.name);
            enemiesInRange.Add(other.gameObject);
            if (!isFiring)
            {
                isFiring = true;
                Debug.Log("Starting firing coroutine");
                StartCoroutine(Firing());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("OnTriggerExit: " + other.gameObject.name);
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            Debug.Log("Enemy left range, enemies remaining: " + enemiesInRange.Count);
        }
    }

    private IEnumerator Firing()
    {
        Debug.Log("Firing coroutine started");
        float elapsedTime = 0f;
        while (elapsedTime < trapDuration)
        {
            enemiesInRange.RemoveAll(e => e == null);

            if (enemiesInRange.Count == 0)
            {
                Debug.Log("No enemies in range, waiting...");
                yield return new WaitUntil(() => enemiesInRange.Count > 0);
                Debug.Log("Enemy entered range, resuming fire");
                elapsedTime = 0f;
            }

            if (bullets.Count > 0)
            {
                Debug.Log("Firing bullet! Bullets remaining: " + bullets.Count);
                GameObject randomFirePoint = firePoints[Random.Range(0, firePoints.Length)];
                GameObject firedBullet = bullets[Random.Range(0, bullets.Count)];
                firedBullet.transform.position = randomFirePoint.transform.position;
                firedBullet.SetActive(true);
                Bullet bulletScript = firedBullet.GetComponent<Bullet>();
                bulletScript.parentTrap = this;
                bulletScript.damage = damage;
                bulletScript.rb.AddForce(randomFirePoint.transform.forward * bulletSpeed, ForceMode.Impulse);
                bullets.Remove(firedBullet);

                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                Debug.Log("No bullets in pool!");
                yield return null;
            }

            elapsedTime += Time.deltaTime;
        }

        Debug.Log("Trap duration ended, stopping fire");
        isFiring = false;
    }
}