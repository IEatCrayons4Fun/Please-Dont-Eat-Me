using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Gun : MonoBehaviour, IWeapon
{
    [Header("Gun Stats")]
    public string weaponName = "Pistol";
    [SerializeField] private float damage = 25f;
    [SerializeField] private float range = 100f;
    [SerializeField] private float fireRate = 0.25f;
    [SerializeField] private LayerMask ignoreLayers;

    [Header("Ammo")]
    public int maxAmmo = 30;
    public int currentAmmo;

    // IWeapon interface
    string IWeapon.weaponName => weaponName;
    int IWeapon.currentAmmo => currentAmmo;
    int IWeapon.maxAmmo => maxAmmo;

    [Header("Bullet Hit Effects")]
    [SerializeField] private GameObject hitParticlePrefab;
    [SerializeField] private LineRenderer bulletTrail;
    [SerializeField] private float lineDuration = 0.05f;
    [SerializeField] private Transform Barrel;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Pickup Transform")]
    public Vector3 localPositionOffset;
    public Vector3 localRotationOffset;


    private float lastFireTime = 0f;
    private InputAction attack;
    private InputAction reload;
    private bool isPickedUp = false;

    void Start()
    {
        attack = InputSystem.actions.FindAction("Attack");
        reload = InputSystem.actions.FindAction("Reload");
        currentAmmo = maxAmmo;
        if (muzzleFlash != null) muzzleFlash.Stop();
    }

    void Update()
    {
        if (!isPickedUp) return;

        //Testing Offset
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localRotationOffset);
        
        if (attack.WasPressedThisFrame() && Time.time >= lastFireTime + fireRate)
        {
            if (currentAmmo <= 0)
            {
                Debug.Log("[Gun] Out of ammo!");
                return;
            }
            lastFireTime = Time.time;
            Shoot();
        }

        if (reload.WasPressedThisFrame())
            Reload();
    }

    private void Shoot()
    {
        currentAmmo--;
        WeaponManagment.instance.UpdateAmmoUI(currentAmmo, maxAmmo);

        if (muzzleFlash != null) muzzleFlash.Play();

        Ray ray = new Ray(CameraSingleton.instance.transform.position, CameraSingleton.instance.transform.forward);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit, range, ~ignoreLayers);

        Vector3 endPoint = didHit ? hit.point : ray.origin + ray.direction * range;
        StartCoroutine(BulletTrail(Barrel.position, endPoint));

        if (didHit)
        {
            if (hitParticlePrefab != null)
            {
                GameObject fx = Instantiate(hitParticlePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(fx, 1f);
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

    private void Reload()
    {
        currentAmmo = maxAmmo;
        WeaponManagment.instance.UpdateAmmoUI(currentAmmo, maxAmmo);
        Debug.Log($"[Gun] Reloaded {weaponName}");
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        WeaponManagment.instance.UpdateAmmoUI(currentAmmo, maxAmmo);
    }

    public void OnPickedUp()
    {
        isPickedUp = true;
        if (muzzleFlash != null) muzzleFlash.Stop();
    }

    public void StopMuzzleFlash()
    {
        if (muzzleFlash != null) muzzleFlash.Stop();
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