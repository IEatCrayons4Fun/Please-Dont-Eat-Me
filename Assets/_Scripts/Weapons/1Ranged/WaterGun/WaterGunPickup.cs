using UnityEngine;

public class WaterGunPickup : MonoBehaviour, IInteractable
{
    private WaterGun waterGun;

    private void Awake()
    {
        waterGun = GetComponent<WaterGun>();
    }

    public void Interacted()
    {
        WeaponManagment.instance.PickupWaterGun(waterGun);
    }
}