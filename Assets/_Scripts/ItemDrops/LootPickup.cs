using UnityEngine;

public class LootPickup : MonoBehaviour, IInteractable
{
    public enum LootType { Ammo, Health, Grenade }
    public LootType lootType;
    public int amount = 10;
    [HideInInspector] public GameObject linkedEffect;

    public void Interacted()
    {
        Debug.Log("Tryed Interacting");
        Debug.Log($"[LootPickup] Picked up {lootType} x{amount}");

        if (linkedEffect != null)
        {
            Debug.Log("[LootPickup] Destroying linked effect");
            Destroy(linkedEffect);
        }
        else
        {
            Debug.LogWarning("[LootPickup] No linked effect found");
        }

        Destroy(gameObject);
    }
}