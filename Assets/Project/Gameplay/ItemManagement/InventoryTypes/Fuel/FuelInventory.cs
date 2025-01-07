using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Fuel
{
    public class FuelInventory : Inventory
    {
        [FormerlySerializedAs("FuelItemAllowed")]
        public Inventory primaryPlayerInventory;
        public InventoryItem fuelItemAllowed; // The base inventory item
        public MMFeedbacks fuelStartsFeedback;
        public MMFeedbacks fuelEndsFeedback;
        // Shows what fraction of the top fuel item unit has been burnt
        public MMProgressBar fuelBurntProgressBar;
        public bool IsBurning;
        public string cookingStationID;
        public float updateInterval = 0.1f;

        public override bool AddItem(InventoryItem fuelItem, int quantity)
        {
            Debug.Log("FuelInventory.AddItem");
            if (fuelItem.ItemID == fuelItemAllowed.ItemID)
            {
                fuelStartsFeedback?.PlayFeedbacks();
                var fuelItemInstance = new FuelItem(fuelItem);
                StartCoroutine(BurnFuel(fuelItemInstance, quantity));
                return base.AddItem(fuelItem, quantity);
            }

            Debug.Log("FuelInventory.AddItem: Item not allowed");
            return false;
        }

        IEnumerator BurnFuel(FuelItem fuelItem, int quantity)
        {
            IsBurning = true;
            float elapsedTime = 0;

            MMGameEvent.Trigger("BurnFuel", stringParameter: cookingStationID);

            fuelBurntProgressBar.BarProgress = fuelItem.remainingFraction;


            while (elapsedTime < fuelItem.burnDuration)
            {
                fuelItem.remainingFraction = 1 - elapsedTime / fuelItem.burnDuration;


                fuelBurntProgressBar.UpdateBar(fuelItem.remainingFraction, 0, 1);


                // Debug.Log("FuelInventory.BurnFuel: " + fuelItem.remainingFraction);

                yield return null;

                elapsedTime += Time.deltaTime;
            }

            RemoveItem(0, 1);

            fuelEndsFeedback?.PlayFeedbacks();

            MMGameEvent.Trigger("SpentFuel", stringParameter: cookingStationID);
            IsBurning = false;
            // fuelItems[0].burnDuration = fuelItems[0].remainingFraction * fuelItems[0].burnDuration;
            // StartCoroutine(BurnFuel(fuelItems[0], 1));
        }

        public override bool AddItemAt(InventoryItem fuelItem, int quantity, int index)
        {
            Debug.Log("FuelInventory.AddItemAt");
            if (fuelItem.ItemID == fuelItemAllowed.ItemID)
            {
                fuelStartsFeedback?.PlayFeedbacks();
                var fuelItemInstance = new FuelItem(fuelItem);
                StartCoroutine(BurnFuel(fuelItemInstance, quantity));
                return base.AddItemAt(fuelItem, quantity, index);
            }

            // Add item to primary inventory if not allowed 
            if (primaryPlayerInventory.AddItem(fuelItem, quantity)) return true;

            Debug.Log("FuelInventory.AddItemAt: Item not allowed");
            return false;
        }

        public override bool MoveItem(int oldIndex, int newIndex)
        {
            return true;
        }
    }
}
