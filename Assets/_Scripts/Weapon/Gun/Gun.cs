using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Gun")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private GameObject[] firePoints;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int maxBulletCount = 13;
    [SerializeField] [Range(1f, 100f)] private float bulletSpeed = 40f;
    private InputAction attack;
     private bool isFiring = false;
    

    [HideInInspector] public List<GameObject> bullets = new List<GameObject>();

    private void Start()
    {
        attack = InputSystem.actions.FindAction("Attack");

        if (bulletPrefab == null)
        {
            Debug.LogError("Gun: bulletPrefab is null");
            return;
        }

        if (firePoints == null || firePoints.Length == 0)
        {
            Debug.LogError("Gun: no firePoints assigned");
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

    private void Update(){
        if(attack.triggered){
            StartCoroutine(Firing());
        }
    }

    private IEnumerator Firing()
    {
        while (true)
        {

            if (firePoints == null || firePoints.Length == 0)
            {
                isFiring = false;
                yield break;
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
                    bulletScript.parentTrap = this;
                    bulletScript.damage = damage;
                    bulletScript.OnFired(direction, bulletSpeed);
                }

                bullets.Remove(firedBullet);
            

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
