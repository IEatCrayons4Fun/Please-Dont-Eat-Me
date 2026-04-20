using UnityEngine;

public class LootPickup : MonoBehaviour, IInteractable
{
    public enum LootType { Ammo, Health, Grenade }
    public LootType lootType;
    public int amount = 10;
    [SerializeField] float healAmount;
    [HideInInspector] public GameObject linkedEffect;

    [SerializeField] private GameObject player;

    private void Start(){
        player = PlayerSingleton.instance.gameObject;
    }

    public void Interacted()
    {
        
        Debug.Log($"[LootPickup] Picked up {lootType} x{amount}");
        ItemEffect();

        if (linkedEffect != null)
        {
            Destroy(linkedEffect);
        }
        Destroy(gameObject);
    }

    private void ItemEffect()
    {
        if(lootType.ToString() == "Health"){
            Debug.Log("Testing Health Pickup");
            player.GetComponent<HealthManager>().Heal(amount);
        }
        else if(lootType.ToString() == "Ammo"){
            Debug.Log("Testing Ammo Pickup");
            player.GetComponent<WeaponManagment>().AddAmmo(amount);
        }
        else if(lootType.ToString() == "Grenade"){
            Debug.Log("Testing Grenade Pickup");
            player.GetComponent<WeaponManagment>().AddGrenade();
        }
    }
}