using System;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.ItemManagement.InventoryItemTypes.Consumables
{
    [CreateAssetMenu(
        fileName = "InventoryKinemaPotion", menuName = "Roguelike/Items/KinemaPotion", order = 1)]
    [Serializable]
    public class MutableStatRecoveryPotionItem : InventoryItem
    {
        [FormerlySerializedAs("PointsToAwardSelectedStat")]
        public float pointsToAwardSelectedStat = 20;

        public override bool Use(string playerID)
        {
            base.Use(playerID);

            throw new NotImplementedException();
        }
    }
}
