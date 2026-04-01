using UnityEngine;

public class HealthPickup : PickupBase
{
    [Header("Health Pickup")]
    [SerializeField] float healAmount;
    public override void PickupEffect()
    {
        HealthManager healthManager = player.GetComponent<HealthManager>();
        if (healthManager != null)
        {
            healthManager.Heal(healAmount);
        }
    }
}
