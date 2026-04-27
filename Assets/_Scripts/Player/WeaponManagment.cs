using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Aegis.GrenadeSystem.HiEx;

public class WeaponManagment : MonoBehaviour
{
    public static WeaponManagment instance;

    [Header("Weapon Holder")]
    [SerializeField] private Transform weaponHolder;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI weaponNameText;

    private List<IWeapon> inventory = new List<IWeapon>();
    private int currentIndex = -1;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (ammoText != null) ammoText.gameObject.SetActive(false);
        if (weaponNameText != null) weaponNameText.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleWeaponSwitch();
    }

    private void HandleWeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && inventory.Count >= 1) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2) && inventory.Count >= 2) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3) && inventory.Count >= 3) EquipWeapon(2);
    }

    // ─── Pickup ───────────────────────────────────────────────────

    public bool PickupWeapon(Gun gun)
    {
        foreach (IWeapon w in inventory)
        {
            if (w.weaponName == gun.weaponName)
            {
                w.AddAmmo(gun.maxAmmo);
                Destroy(gun.gameObject);
                Debug.Log($"[WeaponManagment] Already have {gun.weaponName}, added ammo.");
                return false;
            }
        }

        if (inventory.Count >= 3)
        {
            Debug.Log("[WeaponManagment] Inventory full (3 weapons max).");
            return false;
        }

        AttachToHolder(gun.gameObject);
        gun.OnPickedUp();
        inventory.Add(gun);

        ShowUI();
        EquipWeapon(inventory.Count - 1);
        Debug.Log($"[WeaponManagment] Picked up {gun.weaponName}");
        return true;
    }

    public bool PickupWaterGun(WaterGun gun)
    {
        foreach (IWeapon w in inventory)
        {
            if (w.weaponName == gun.weaponName)
            {
                w.AddAmmo(gun.maxAmmo);
                Destroy(gun.gameObject);
                Debug.Log($"[WeaponManagment] Already have {gun.weaponName}, added ammo.");
                return false;
            }
        }

        if (inventory.Count >= 3)
        {
            Debug.Log("[WeaponManagment] Inventory full (3 weapons max).");
            return false;
        }

        AttachToHolder(gun.gameObject);
        gun.OnPickedUp();
        inventory.Add(gun);

        ShowUI();
        EquipWeapon(inventory.Count - 1);
        Debug.Log($"[WeaponManagment] Picked up {gun.weaponName}");
        return true;
    }

    // ─── Equip ────────────────────────────────────────────────────

    private void EquipWeapon(int index)
    {
        DeactivateAll();

        currentIndex = index;
        IWeapon current = inventory[currentIndex];

        GameObject go = GetWeaponObject(current);
        if (go != null)
        {
            go.SetActive(true);
            current.StopMuzzleFlash();
        }

        UpdateAmmoUI(current.currentAmmo, current.maxAmmo);
        if (weaponNameText != null) weaponNameText.text = current.weaponName;
    }

    private void DeactivateAll()
    {
        foreach (IWeapon w in inventory)
        {
            GameObject go = GetWeaponObject(w);
            if (go != null)
            {
                w.StopMuzzleFlash();
                go.SetActive(false);
            }
        }
    }

    // ─── Helpers ──────────────────────────────────────────────────

    private void AttachToHolder(GameObject go)
    {
        go.transform.SetParent(weaponHolder);

        // Check for Gun offset
        Gun gun = go.GetComponent<Gun>();
        if (gun != null)
        {
            go.transform.localPosition = gun.localPositionOffset;
            go.transform.localRotation = Quaternion.Euler(gun.localRotationOffset);
        }

        // Check for WaterGun offset
        WaterGun waterGun = go.GetComponent<WaterGun>();
        if (waterGun != null)
        {
            go.transform.localPosition = waterGun.localPositionOffset;
            go.transform.localRotation = Quaternion.Euler(waterGun.localRotationOffset);
        }

        Rigidbody rb = go.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider col = go.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        go.SetActive(false);
    }

    private GameObject GetWeaponObject(IWeapon weapon)
    {
        if (weapon is Gun gun) return gun.gameObject;
        if (weapon is WaterGun wg) return wg.gameObject;
        return null;
    }

    private void ShowUI()
    {
        if (ammoText != null) ammoText.gameObject.SetActive(true);
        if (weaponNameText != null) weaponNameText.gameObject.SetActive(true);
    }

    // ─── Ammo ─────────────────────────────────────────────────────

    public void UpdateAmmoUI(int current, int max)
    {
        if (ammoText != null)
            ammoText.text = $"{current} / {max}";
    }

    public void AddAmmo(int amount)
    {
        if (currentIndex < 0 || inventory.Count == 0) return;
        inventory[currentIndex].AddAmmo(amount);
    }

    // ─── Grenade ──────────────────────────────────────────────────

    public void AddGrenade()
    {
        GetComponent<GrenadeSystem>().PickupGrenade();
    }
}