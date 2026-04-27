using UnityEngine;
using System.Collections.Generic;

public class LootDropper : MonoBehaviour
{
    public LootTable lootTable;
    public float dropHeight = 0.5f;
    public float spreadRadius = 1.5f;
    public GameObject dropEffect;

    public void DropLoot()
    {
        if (lootTable == null || lootTable.entries.Length == 0) return;

        var toDrop = new List<LootTable.LootEntry>();

        foreach (var entry in lootTable.entries)
        {
            if (entry.prefab == null) continue;
            if (Random.Range(0f, 100f) <= entry.dropChance)
                toDrop.Add(entry);
        }

        for (int i = 0; i < toDrop.Count; i++)
        {
            Vector3 offset = Vector3.zero;

            if (toDrop.Count == 1)
            {
                offset = new Vector3(spreadRadius, 0f, 0f);
            }
            else
            {
                float angle = i * (360f / toDrop.Count);
                float rad = angle * Mathf.Deg2Rad;
                offset = new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * spreadRadius;
            }

            Vector3 spawnPos = transform.position + Vector3.up * dropHeight + offset;

            GameObject loot = Instantiate(toDrop[i].prefab, spawnPos, Quaternion.identity);

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