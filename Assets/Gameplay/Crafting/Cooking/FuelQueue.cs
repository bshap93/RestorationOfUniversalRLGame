using System;
using System.Collections.Generic;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Gameplay.Crafting.Cooking
{
    [Serializable]
    public class FuelQueue : MonoBehaviour
    {
        public string requiredFuelItemID;


        List<InventoryItem> _fuelQueue;

        public FuelQueue()
        {
            _fuelQueue = new List<InventoryItem>();
        }

        public void AddFuelItem(InventoryItem fuelItem)
        {
            if (fuelItem.ItemID == requiredFuelItemID) _fuelQueue.Add(fuelItem);
        }


        public bool hasValidFuel()
        {
            return _fuelQueue.Count > 0;
        }
    }
}
