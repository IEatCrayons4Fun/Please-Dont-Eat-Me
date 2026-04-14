using UnityEngine;

public class LootDropper : MonoBehaviour
{
    public LootTable lootTable;
    public float dropHeight = 0.5f;
    public GameObject dropEffect;

    public void DropLoot()
    {
        Debug.Log($"[LootDropper] DropLoot called on {gameObject.name}");

        if (lootTable == null)
        {
            Debug.LogError("[LootDropper] LootTable is NULL - assign it in the inspector!");
            return;
        }

        Debug.Log($"[LootDropper] LootTable has {lootTable.entries.Length} entries");

        foreach (var entry in lootTable.entries)
        {
            float roll = Random.value;
            Debug.Log($"[LootDropper] Rolling for {entry.prefab?.name ?? "NULL PREFAB"} | Roll: {roll:F2} | Chance: {entry.dropChance:F2}");

            if (entry.prefab == null)
            {
                Debug.LogError("[LootDropper] A loot entry has a NULL prefab - assign it in the inspector!");
                continue;
            }

            if (roll <= entry.dropChance)
            {
                Vector3 spawnPos = transform.position + Vector3.up * dropHeight;
                GameObject loot = Instantiate(entry.prefab, spawnPos, Quaternion.identity);
                Debug.Log($"[LootDropper] Spawned {entry.prefab.name} at {spawnPos}");

                if (dropEffect != null)
                {
                    GameObject effect = Instantiate(dropEffect, spawnPos, Quaternion.identity);
                    LootPickup pickup = loot.GetComponent<LootPickup>();
                    if (pickup != null)
                        pickup.linkedEffect = effect;
                    else
                        Debug.LogWarning($"[LootDropper] {entry.prefab.name} has no LootPickup component!");
                }
                else
                {
                    Debug.LogWarning("[LootDropper] No drop effect assigned");
                }
            }
            else
            {
                Debug.Log($"[LootDropper] {entry.prefab.name} did not drop this time");
            }
        }
    }
}