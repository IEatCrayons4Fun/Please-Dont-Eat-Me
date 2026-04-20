using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    private Gun gun;

    private void Awake()
    {
        gun = GetComponent<Gun>();
    }

    public void Interacted()
    {
        WeaponManagment.instance.PickupWeapon(gun);
    }
}