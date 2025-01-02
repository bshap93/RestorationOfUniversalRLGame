using System;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryConsumable", menuName = "Roguelike/Items/InventoryConsumable", order = 2)]
    [Serializable]
    public class InventoryConsumable : PreviewableInventoryItem
    {
        public float HealthToGive = 4; // Amount of health to recover

        public override bool Use(string playerID)
        {
            base.Use(playerID);

            // Get Player1 character
            var character = TargetInventory(playerID)?.Owner?.GetComponent<Character>();

            if (character != null)
            {
                var characterHealth = character.gameObject.GetComponent<Health>();

                if (characterHealth != null)
                {
                    characterHealth.ReceiveHealth(HealthToGive, character.gameObject);
                    return true; // Indicates successful use
                }
            }

            return false; // Use was not successful
        }
    }
}
