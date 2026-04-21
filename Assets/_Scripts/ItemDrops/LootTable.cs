using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Zombie Game/Loot Table")]
public class LootTable : ScriptableObject
{
    [System.Serializable]
    public class LootEntry
    {
        public GameObject prefab;
        [Range(0f, 100f)] public float dropChance;
    }

    public LootEntry[] entries;
}