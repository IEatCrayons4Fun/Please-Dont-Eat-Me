using UnityEngine;

public interface IWeapon
{
    string weaponName { get; }
    int currentAmmo { get; }
    int maxAmmo { get; }
    void OnPickedUp();
    void StopMuzzleFlash();
    void AddAmmo(int amount);
}
