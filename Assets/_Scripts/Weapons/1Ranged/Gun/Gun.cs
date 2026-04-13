using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.25f;

    [SerializeField] private LayerMask ignoreLayers;

    [Header("References")]
    [SerializeField] private GameObject hitParticlePrefab;

    private float lastFireTime = 0f;
    private InputAction attack;
    private Vector3 lastHitPoint;
    private bool didHit = false;

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

        Ray ray = new Ray(CameraSingleton.instance.transform.position, CameraSingleton.instance.transform.forward);

        didHit = Physics.Raycast(ray, out RaycastHit hit, range, ~ignoreLayers);

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
                ZombieHealth health = hit.collider.GetComponent<ZombieHealth>();
                if (health != null)
                    health.TakeDamage(damage);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (CameraSingleton.instance == null) return;

        Gizmos.color = didHit ? Color.green : Color.red;
        Vector3 endPoint = didHit ? lastHitPoint : CameraSingleton.instance.transform.position + CameraSingleton.instance.transform.forward * range;
        Gizmos.DrawLine(CameraSingleton.instance.transform.position, endPoint);
    }
}