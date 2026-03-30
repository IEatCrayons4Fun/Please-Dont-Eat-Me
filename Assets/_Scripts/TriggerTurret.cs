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
    public List<GameObject> enemiesInRange = new List<GameObject>();
    private bool isFiring = false;


    private void Start()
    {
        StartCoroutine(SpawnBullets());
    }

    private IEnumerator SpawnBullets()
    {
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
                bulletScript.parentTrap = this;
                bulletScript.damage = damage;
                bulletScript.Fire(randomFirePoint.transform.forward, bulletSpeed);
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