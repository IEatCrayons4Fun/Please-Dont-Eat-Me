using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TriggerTurret : TrapBase
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

    private void Start()
    {
        StartCoroutine(SpawnBullets());
    }

    private IEnumerator SpawnBullets()
    {
        while(bullets.Count < maxBulletCount)
        {
            GameObject bullet = Instantiate(bulletPrefab, this.transform);
            bullets.Add(bullet);
            bullet.SetActive(false);
        }
        yield return null;
    }
    public override void TriggerTrap()
    {
        StartCoroutine(Firing());
    }

    private IEnumerator Firing()
    {
        float elapsedTime = 0f;
        while(elapsedTime < trapDuration)
        {
            if(canFire && arrows.Count > 0){
                StartCoroutine(FireDelay());
                GameObject randomFirePoint = firePoints[Random.Range(0, firePoints.Length)];
                GameObject firedArrow = arrows[Random.Range(0, arrows.Count - 1)];
                firedArrow.transform.position = randomFirePoint.transform.position;
                firedArrow.SetActive(true);
                Bullet bulletScript = firedArrow.GetComponent<Bullet>();
                bulletScript.parentTrap = this;
                bulletScript.damage = damage;
                bulletScript.rb.AddForce(randomFirePoint.transform.forward * bulletSpeed, ForceMode.Impulse);
                bullets.Remove(firedArrow);
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    private IEnumerator FireDelay()
    {
        canFire = false;
        yield return new WaitForSeconds(fireRate);
        canFire = true;
    }
}
