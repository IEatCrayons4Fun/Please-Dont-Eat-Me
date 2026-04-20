using UnityEngine;
using UnityEngine.InputSystem;

public class WaterGun : MonoBehaviour, IWeapon
{
    [Header("Water Gun Stats")]
    public string weaponName = "Water Gun";
    [SerializeField] private float damage = 5f;
    [SerializeField] private float range = 15f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private LayerMask ignoreLayers;

    [Header("Ammo")]
    public int maxAmmo = 100;
    public int currentAmmo;

    // IWeapon interface
    string IWeapon.weaponName => weaponName;
    int IWeapon.currentAmmo => currentAmmo;
    int IWeapon.maxAmmo => maxAmmo;

    [Header("Slow Settings")]
    [SerializeField] private float slowMultiplier = 0.4f;
    [SerializeField] private float slowDuration = 2f;

    [Header("VFX")]
    [SerializeField] private GameObject waterBeam;
    [SerializeField] private Transform barrel;

    [Header("Pickup Transform")]
    public Vector3 localPositionOffset;
    public Vector3 localRotationOffset;

    private float lastFireTime = 0f;
    private InputAction attack;
    private InputAction reload;
    private bool isPickedUp = false;
    private bool isFiring = false;
    private Zombie currentSlowedZombie = null;
    private ParticleSystem[] beamParticles;

    void Start()
    {
        attack = InputSystem.actions.FindAction("Attack");
        reload = InputSystem.actions.FindAction("Reload");
        currentAmmo = maxAmmo;

        if (waterBeam != null)
        {
            waterBeam.transform.SetParent(null);
            beamParticles = waterBeam.GetComponentsInChildren<ParticleSystem>(true);
            waterBeam.transform.position = new Vector3(0f, -9999f, 0f);
            StopBeamParticles();
        }
    }

    void Update()
    {
        if (!isPickedUp) return;

        //Live Testing Offset
        transform.localPosition = localPositionOffset;
        transform.localRotation = Quaternion.Euler(localRotationOffset);

        bool triggerHeld = attack.IsPressed() && currentAmmo > 0;

        if (triggerHeld)
        {
            if (!isFiring)
            {
                isFiring = true;
                PlayBeamParticles();
            }

            if (Time.time >= lastFireTime + fireRate)
            {
                lastFireTime = Time.time;
                Shoot();
            }

            AimBeam();
        }
        else
        {
            if (isFiring)
                StopBeam();
        }

        if (reload.WasPressedThisFrame())
            Reload();

        if (currentAmmo <= 0 && isFiring)
            StopBeam();
        
        
    }

    private void Shoot()
    {
        currentAmmo--;
        WeaponManagment.instance.UpdateAmmoUI(currentAmmo, maxAmmo);

        Ray ray = new Ray(CameraSingleton.instance.transform.position,
                          CameraSingleton.instance.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, ~ignoreLayers))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                ZombieHP health = hit.collider.GetComponent<ZombieHP>();
                if (health != null) health.TakeDamage(damage);

                Zombie ai = hit.collider.GetComponent<Zombie>();
                if (ai != null)
                {
                    ai.ApplySlowIndefinite(slowMultiplier);
                    currentSlowedZombie = ai;
                }
            }

            NPCHealth npcHealth = hit.collider.GetComponent<NPCHealth>();
            if (npcHealth != null) npcHealth.TakeDamage(damage);
        }
    }

    private void AimBeam()
    {
        if (waterBeam == null || barrel == null) return;

        waterBeam.transform.position = barrel.position;

        Vector3 targetPoint = CameraSingleton.instance.transform.position
                            + CameraSingleton.instance.transform.forward * range;

        Vector3 direction = (targetPoint - barrel.position).normalized;
        waterBeam.transform.rotation = Quaternion.LookRotation(direction)
                                    * Quaternion.Euler(-90f, 0f, 0f);
    }

    private void PlayBeamParticles()
    {
        if (beamParticles == null) return;
        foreach (ParticleSystem ps in beamParticles)
        {
            if (ps.gameObject.name == "beam")
            {
                var main = ps.main;
                main.loop = true;
            }
            ps.Play();
        }
    }

    private void StopBeamParticles()
    {
        if (beamParticles == null) return;
        foreach (ParticleSystem ps in beamParticles)
        {
            if (ps != null)
                ps.Stop();
        }
    }

    private void StopBeam()
    {
        isFiring = false;
        StopBeamParticles();
        if (waterBeam != null)
            waterBeam.transform.position = new Vector3(0f, -9999f, 0f);

        if (currentSlowedZombie != null)
        {
            currentSlowedZombie.StartSlowCountdown(slowDuration);
            currentSlowedZombie = null;
        }
    }

    private void Reload()
    {
        currentAmmo = maxAmmo;
        WeaponManagment.instance.UpdateAmmoUI(currentAmmo, maxAmmo);
        Debug.Log("[WaterGun] Refilled!");
    }

    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + amount, 0, maxAmmo);
        WeaponManagment.instance.UpdateAmmoUI(currentAmmo, maxAmmo);
    }

    public void OnPickedUp()
    {
        isPickedUp = true;
        if (waterBeam != null)
        {
            waterBeam.transform.position = new Vector3(0f, -9999f, 0f);
            StopBeamParticles();
        }
    }

    public void StopMuzzleFlash()
    {
        StopBeam();
    }

    private void OnDisable()
    {
        if (waterBeam != null)
            waterBeam.transform.position = new Vector3(0f, -9999f, 0f);

        StopBeamParticles();
        isFiring = false;
    }
}