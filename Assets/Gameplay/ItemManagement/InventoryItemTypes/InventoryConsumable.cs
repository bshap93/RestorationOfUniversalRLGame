using System;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryConsumable", menuName = "Roguelike/Items/InventoryConsumable", order = 2)]
    [Serializable]
    public class InventoryConsumable : BaseItem
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
