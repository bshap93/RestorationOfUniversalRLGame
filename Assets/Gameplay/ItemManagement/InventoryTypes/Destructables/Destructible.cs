using System.Collections.Generic;
using JetBrains.Annotations;
using Project.Gameplay.Interactivity;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryTypes.Destructables
{
    /// </summary>
    [CreateAssetMenu(
        fileName = "Crafting", menuName = "Crafting/Destructable", order = 1)]
    public class Destructible : Interactable
    {
        [Header("Prefabs")] public GameObject destroyedPrefab; // List of possible destroyed prefabs
        public List<GameObject> intermediatePrefabs;

        [Header("Health")] public float maxHealth = 30f;
        public List<float> intermediateHealthThresholds;
        public Vector2Int dropAmountRange = new(1, 3); // Randomized loot drop amount

        [Header("Loot")] [CanBeNull] public List<GameObject> possibleLoot; // Multiple possible loot prefabs
    }
}
