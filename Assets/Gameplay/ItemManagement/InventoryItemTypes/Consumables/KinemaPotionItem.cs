using System;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.Magic;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryItemTypes.Consumables
{
    [CreateAssetMenu(
        fileName = "InventoryKinemaPotion", menuName = "Roguelike/Items/KinemaPotion", order = 1)]
    [Serializable]
    public class KinemaPotionItem : InventoryItem
    {
        public float KinemaToGive = 20;

        public override bool Use(string playerID)
        {
            base.Use(playerID);

            // Get Player1 character
            var character = TargetInventory(playerID)?.Owner?.GetComponent<Character>();

            if (character != null)
            {
                var characterMagicSystem = character.gameObject.GetComponent<MagicSystem>();

                if (characterMagicSystem != null)
                {
                    characterMagicSystem.ReceiveKinema(KinemaToGive, character.gameObject);
                    return true; // Indicates successful use
                }
            }

            return false; // Use was not successful
        }
    }
}
