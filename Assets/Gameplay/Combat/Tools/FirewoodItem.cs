using System;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.Combat.Tools
{
    [CreateAssetMenu(fileName = "FirewoodItem", menuName = "Inventory/Equipment/Firewood", order = 2)]
    [Serializable]
    public class FirewoodItem : InventoryItem
    {
        [Header("Torch Properties")] [Tooltip("The torch prefab to instantiate when equipped.")]
        public GameObject CampfirePrefab;


        public void BuildSmallFire(Vector3 position)
        {
            Instantiate(CampfirePrefab, position, Quaternion.identity);
            // TODO: Add a small fire to the scene and be able to interact with it
        }
    }
}
