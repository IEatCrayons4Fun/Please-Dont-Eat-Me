using UnityEngine;

public class LootDropper : MonoBehaviour
{
    public LootTable lootTable;
    public float dropHeight = 0.5f;
    public GameObject dropEffect;

    public void DropLoot()
    {
        if (lootTable == null)
        {
            return;
        }

        foreach (var entry in lootTable.entries)
        {
            float roll = Random.value;

            if (entry.prefab == null)
            {
                continue;
            }

            if (roll <= entry.dropChance)
            {
                Vector3 spawnPos = transform.position + Vector3.up * dropHeight;
                GameObject loot = Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                

                if (dropEffect != null)
                {
                    GameObject effect = Instantiate(dropEffect, spawnPos, Quaternion.identity);
                    LootPickup pickup = loot.GetComponent<LootPickup>();
                    if (pickup != null)
                        pickup.linkedEffect = effect;
                }
            }
        }
    }
}