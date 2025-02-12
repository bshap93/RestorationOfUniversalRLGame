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
        
        public int maxFuel = 5; // Maximum fuel items that can be stored


        List<InventoryItem> _fuelQueue;

        public FuelQueue()
        {
            _fuelQueue = new List<InventoryItem>();
        }

        public void AddFuelItem(InventoryItem fuelItem)
        {
            if (fuelItem.ItemID == requiredFuelItemID && _fuelQueue.Count < maxFuel) 
            {
                _fuelQueue.Add(fuelItem);
                OnFuelChanged?.Invoke(GetFuelPercentage());
            }
        }
        


        // Get fuel level as a percentage (0-100)
        public float GetFuelPercentage()
        {
            return (_fuelQueue.Count / (float)maxFuel) * 100f;
        }


        public bool hasValidFuel()
        {
            return _fuelQueue.Count > 0;
        }
        
        // Event for when fuel amount changes
        public event Action<float> OnFuelChanged;

        // Consume one fuel unit
        public bool ConsumeFuel()
        {
            if (!hasValidFuel()) return false;
            
            _fuelQueue.RemoveAt(0);
            OnFuelChanged?.Invoke(GetFuelPercentage());
            return true;
        }
    }
}
