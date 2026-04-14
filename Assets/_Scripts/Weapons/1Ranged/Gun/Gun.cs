using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.25f;

    [SerializeField] private LayerMask ignoreLayers;

    [Header("Bullet Hit Effects")]
    [SerializeField] private GameObject hitParticlePrefab;

    private float lastFireTime = 0f;
    private InputAction attack;
    private Vector3 lastHitPoint;
    private bool didHit = false;

    [SerializeField] private LineRenderer bulletTrail;
    [SerializeField] private float lineDuration = 0.05f;

    [SerializeField] private Transform Barrel;
    [SerializeField] private ParticleSystem muzzleFlash;

    void Start()
    {
        attack = InputSystem.actions.FindAction("Attack");
    }

    void Update()
    {

        if (attack.WasPressedThisFrame() && Time.time >= lastFireTime + fireRate)
        {
            didHit = false;
            lastFireTime = Time.time;
            Shoot();
        }
    }

    private void Shoot()
    {
        if (muzzleFlash != null) muzzleFlash.Play();

        Ray ray = new Ray(CameraSingleton.instance.transform.position, CameraSingleton.instance.transform.forward);
        didHit = Physics.Raycast(ray, out RaycastHit hit, range, ~ignoreLayers);

        Vector3 endPoint = didHit ? hit.point : ray.origin + ray.direction * range;
        StartCoroutine(BulletTrail(Barrel.position, endPoint));

        if (didHit)
        {
            lastHitPoint = hit.point;

            if (hitParticlePrefab != null)
            {
                
                GameObject fx = Instantiate(hitParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 2f);
            }

            if (hit.collider.CompareTag("Enemy"))
            {
                ZombieHP health = hit.collider.GetComponent<ZombieHP>();
                if (health != null) health.TakeDamage(damage);

            }

            NPCHealth npcHealth = hit.collider.GetComponent<NPCHealth>();
            if (npcHealth != null) npcHealth.TakeDamage(damage);
        }
    }

    private IEnumerator BulletTrail(Vector3 start, Vector3 end)
    {
        bulletTrail.enabled = true;
        bulletTrail.SetPosition(0, start);
        bulletTrail.SetPosition(1, end);
        yield return new WaitForSeconds(lineDuration);
        bulletTrail.enabled = false;
    }
}