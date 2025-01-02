using System;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes.Consumables
{
    [CreateAssetMenu(
        fileName = "InventoryHealthPotion", menuName = "Roguelike/Items/HealthPotion", order = 1)]
    [Serializable]
    public class InventoryHealthPotion : InventoryItem
    {
        public float HealthToGive = 20; // Amount of health to recover

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
