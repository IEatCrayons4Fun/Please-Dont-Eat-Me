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
    [SerializeField] [Range(0.9f, 1f)] private float aimThreshold = 0.98f;
    [HideInInspector] public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isFiring = false;

    private TrackingEnemy trackingEnemy;

    private void Start()
    {
        trackingEnemy = GetComponentInChildren<TrackingEnemy>();
        StartCoroutine(SpawnBullets());
    }

    private IEnumerator SpawnBullets()
    {
        if (bulletPrefab == null || firePoints == null || firePoints.Length == 0)
        {
            yield break;
        }

        bullets.Clear();
        while (bullets.Count < maxBulletCount)
        {
            GameObject bullet = Instantiate(bulletPrefab, this.transform);
            Collider bulletCol = bullet.GetComponent<Collider>();
            if (bulletCol != null) bulletCol.enabled = false;
            bullets.Add(bullet);
            bullet.SetActive(false);
        }
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
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

    private bool IsAimedAtTarget(GameObject target)
    {
        if (target == null || trackingEnemy == null) return false;

        Vector3 directionToTarget = (target.transform.position - trackingEnemy.transform.position).normalized;
        float dot = Vector3.Dot(trackingEnemy.transform.forward, directionToTarget);
        return dot >= aimThreshold;
    }

    private IEnumerator Firing()
    {
        float elapsedTime = 0f;
        while (elapsedTime < trapDuration)
        {
            enemiesInRange.RemoveAll(e => e == null);

            if (enemiesInRange.Count == 0)
            {
                yield return new WaitUntil(() => enemiesInRange.Count > 0);
                elapsedTime = 0f;
            }

            if (firePoints == null || firePoints.Length == 0)
            {
                yield break;
            }

            GameObject currentTarget = enemiesInRange[0];
            yield return new WaitUntil(() =>
            {
                enemiesInRange.RemoveAll(e => e == null);
                if (enemiesInRange.Count == 0) return true;
                currentTarget = enemiesInRange[0];
                return IsAimedAtTarget(currentTarget);
            });

            if (enemiesInRange.Count == 0)
            {
                yield return new WaitUntil(() => enemiesInRange.Count > 0);
                elapsedTime = 0f;
                continue;
            }

            if (bullets.Count > 0)
            {
                GameObject randomFirePoint = firePoints[Random.Range(0, firePoints.Length)];
                GameObject firedBullet = bullets[Random.Range(0, bullets.Count)];
                firedBullet.transform.position = randomFirePoint.transform.position;
                firedBullet.transform.SetParent(null);
                firedBullet.SetActive(true);
                Collider bulletCol = firedBullet.GetComponent<Collider>();
                if (bulletCol != null) bulletCol.enabled = true;
                Bullet bulletScript = firedBullet.GetComponent<Bullet>();
                if (bulletScript != null)
                {
                    bulletScript.parentTrap = this;
                    bulletScript.damage = damage;
                    bulletScript.Fire(randomFirePoint.transform.forward, bulletSpeed);
                }
                bullets.Remove(firedBullet);

                yield return new WaitForSeconds(fireRate);
            }
            else
            {
                yield return null;
            }

            elapsedTime += Time.deltaTime;
        }

        isFiring = false;
    }
}