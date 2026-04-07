using UnityEngine;

public class HealthPickup : PickupBase
{
    [Header("Health Pickup")]
    [SerializeField] float healAmount;
    public override void PickupEffect(){
        player.GetComponent<HealthManager>().Heal(healAmount);
    }
}
