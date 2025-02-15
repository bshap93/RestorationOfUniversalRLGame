using Gameplay.Player;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.ItemManagement.InventoryItemTypes
{
    [CreateAssetMenu(
        fileName = "InventoryCoinPickup", menuName = "Roguelike/Items/InventoryCoinPickup", order = 1)]
    public class InventoryCoinPickup : BaseItem
    {
        public int MaximumCoins = 10; // Maximum number of coins that can be picked up
        public int MinimumCoins = 1; // Minimum number of coins that can be picked up


        /// <summary>
        ///     This function ensures that if the coin somehow ends up in the inventory, it will automatically be used.
        /// </summary>
        public override bool Use(string playerID)
        {
            base.Use(playerID);

            // Get the player GameObject
            var player = TargetInventory(playerID)?.Owner;

            if (player != null)
            {
                // Access PlayerStats from the player object
                var playerStats = player.GetComponent<PlayerStats>();

                if (playerStats != null)
                {
                    // Randomize the amount of coins to add
                    var coinsToAdd = Random.Range(MinimumCoins, MaximumCoins + 1);
                    playerStats.AddCoins(coinsToAdd);
                    Debug.Log(
                        $"Used coin item and added {coinsToAdd} coins. Total Coins: {playerStats.playerCurrency}");

                    return true; // Indicate successful use
                }
            }

            return false; // Indicate failed use
        }
    }
}
