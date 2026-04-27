using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerTurret : MonoBehaviour, IBulletPoolOwner
{
    [Header("Turret Trap")]
    [SerializeField] [Range(0.1f, 10f)] private float fireRate = 0.5f;
    [SerializeField] [Range(1f, 100f)] private float damage = 10f;
    [SerializeField] private GameObject[] firePoints;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int maxBulletCount = 20;
    [SerializeField] [Range(1f, 100f)] private float bulletSpeed = 40f;
    [SerializeField] [Range(0f, 90f)] private float aimAngle = 30f;

    [HideInInspector] public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isFiring = false;

    private void Start()
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("TriggerTurret: bulletPrefab is null");
            return;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogError("TriggerTurret: no firePoints assigned");
            return;
        }

        StartCoroutine(SpawnBullets());
    }

    private IEnumerator SpawnBullets()
    {
        bullets.Clear();

        for (int i = 0; i < maxBulletCount; i++)
        {
            GameObject b = Instantiate(bulletPrefab, transform);
            b.SetActive(false);

            // disable collider while pooled
            Collider col = b.GetComponent<Collider>();
            if (col != null) col.enabled = false;

            // set up Rigidbody so the pooled object doesn't simulate
            Rigidbody rb = b.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (!rb.isKinematic)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
                rb.isKinematic = true;
                rb.useGravity = false;
                rb.linearDamping = 0f;
                rb.angularDamping = 0f;
            }

            bullets.Add(b);
        }

        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Enemy"))
        {
            
            if (!enemiesInRange.Contains(other.gameObject))
                enemiesInRange.Add(other.gameObject);

            if (!isFiring)
            {
                isFiring = true;
                StartCoroutine(Firing());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
        }
    }

    private bool TryGetFacingTarget(out GameObject firePoint, out GameObject targetEnemy)
    {
        firePoint = null;
        targetEnemy = null;

        if (firePoints == null || firePoints.Length == 0)
            return false;

        List<GameObject> validFirePoints = new List<GameObject>();

        foreach (GameObject fp in firePoints)
        {
            if (fp == null)
                continue;

            foreach (GameObject enemy in enemiesInRange)
            {
                if (enemy == null)
                    continue;

                Vector3 toEnemy = enemy.transform.position - fp.transform.position;
                if (toEnemy == Vector3.zero)
                    continue;

                if (Vector3.Angle(fp.transform.forward, toEnemy) <= aimAngle)
                {
                    validFirePoints.Add(fp);
                    break;
                }
            }
        }

        if (validFirePoints.Count == 0)
            return false;

        firePoint = validFirePoints[Random.Range(0, validFirePoints.Count)];

        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy == null)
                continue;

            Vector3 toEnemy = enemy.transform.position - firePoint.transform.position;
            if (toEnemy == Vector3.zero)
                continue;

            if (Vector3.Angle(firePoint.transform.forward, toEnemy) <= aimAngle)
            {
                targetEnemy = enemy;
                return true;
            }
        }

        return false;
    }

    private IEnumerator Firing()
    {
        while (true)
        {
            enemiesInRange.RemoveAll(e => e == null);

            if (enemiesInRange.Count == 0)
            {
                isFiring = false;
                yield break;
            }

            if (firePoints == null || firePoints.Length == 0)
            {
                isFiring = false;
                yield break;
            }

            // Fire one bullet if available and the turret is looking at a valid target
            if (bullets.Count > 0)
            {
                if (!TryGetFacingTarget(out GameObject firePoint, out GameObject targetEnemy))
                {
                    yield return new WaitForSeconds(fireRate);
                    continue;
                }


                GameObject firedBullet = bullets[Random.Range(0, bullets.Count)];

                // Place and orient the bullet at the muzzle
                float spawnOffset = 0.2f;
                firedBullet.transform.position = firePoint.transform.position + firePoint.transform.forward * spawnOffset;
                // If your bullet faces up (Y+) instead of forward (Z+)
                Quaternion offset = Quaternion.Euler(90f, 0f, 0f);
                firedBullet.transform.rotation =
                    Quaternion.LookRotation(firePoint.transform.forward) * offset;
                firedBullet.transform.SetParent(null);
                firedBullet.SetActive(true);

                Collider bulletCol = firedBullet.GetComponent<Collider>();
                if (bulletCol != null) bulletCol.enabled = true;

                Rigidbody rb = firedBullet.GetComponent<Rigidbody>();
                Vector3 direction = firePoint.transform.forward.normalized;
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.useGravity = false; // no drop
                    rb.linearDamping = 0f;
                    rb.angularDamping = 0f;
                }

                Bullet bulletScript = firedBullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.poolOwner = this;
                    bulletScript.damage = damage;
                    bulletScript.OnFired(direction, bulletSpeed);
                }

                bullets.Remove(firedBullet);
            }

            yield return new WaitForSeconds(fireRate);
        }
    }

    // Called from Bullet to return to the pool
    public void ReturnBulletToPool(GameObject b)
    {
        if (b == null) return;

        Rigidbody rb = b.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Collider col = b.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        b.transform.SetParent(this.transform);
        b.SetActive(false);

        if (!bullets.Contains(b))
            bullets.Add(b);
    }
}