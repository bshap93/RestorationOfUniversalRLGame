using System.Collections;
using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Fuel
{
    public class FuelInventory : BaseInventory
    {
        [FormerlySerializedAs("FuelItemAllowed")]
        public Inventory primaryPlayerInventory;
        public InventoryItem fuelItemAllowed; // The base inventory item
        public MMFeedbacks fuelStartsFeedback;
        public MMFeedbacks fuelEndsFeedback;
        // Shows what fraction of the top fuel item unit has been burnt
        public bool IsBurning;
        public string cookingStationID;
        public float updateInterval = 0.1f;

        public override bool AddItem(InventoryItem fuelItem, int quantity)
        {
            if (fuelItem.ItemID == fuelItemAllowed.ItemID)
            {
                fuelStartsFeedback?.PlayFeedbacks();
                var fuelItemInstance = new FuelItem(fuelItem);
                StartCoroutine(BurnFuel(fuelItemInstance, quantity));
                return base.AddItem(fuelItem, quantity);
            }

            // Add item to primary inventory if not allowed 
            if (primaryPlayerInventory.AddItem(fuelItem, quantity)) return true;


            Debug.Log("FuelInventory.AddItem: Item not allowed");
            return false;
        }

        public void TreatAddedItem(InventoryItem fuelItem, int quantity)
        {
            if (fuelItem.ItemID == fuelItemAllowed.ItemID)
            {
                fuelStartsFeedback?.PlayFeedbacks();
                var fuelItemInstance = new FuelItem(fuelItem);
                StartCoroutine(BurnFuel(fuelItemInstance, quantity));
            }
        }

        IEnumerator BurnFuel(FuelItem fuelItem, int quantity)
        {
            IsBurning = true;
            float elapsedTime = 0;

            MMGameEvent.Trigger("BurnFuel", stringParameter: cookingStationID);

            MMGameEvent.Trigger(
                "UpdateFuelProgressBar", stringParameter: cookingStationID, vector2Parameter: new Vector2(1.0f, 0));


            while (elapsedTime < fuelItem.burnDuration)
            {
                fuelItem.remainingFraction = 1 - elapsedTime / fuelItem.burnDuration;


                // Every second
                if (elapsedTime % 1 < updateInterval)
                    MMGameEvent.Trigger(
                        "UpdateFuelProgressBar",
                        stringParameter: cookingStationID,
                        vector2Parameter: new Vector2(fuelItem.remainingFraction, 0));
                // fuelBurntProgressBar.BarProgress = fuelItem.remainingFraction;

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
            return false;
        }

        public override bool RemoveItem(int index, int quantity)
        {
            Debug.Log("FuelInventory.RemoveItem");
            if (base.RemoveItem(index, quantity))
            {
                fuelEndsFeedback?.PlayFeedbacks();
                return true;
            }

            Debug.Log("FuelInventory.RemoveItem: Item not removed");
            return false;
        }


        public override bool RemoveItemByID(string itemID, int quantity)
        {
            Debug.Log("FuelInventory.RemoveItemByID");
            if (base.RemoveItemByID(itemID, quantity))
            {
                fuelEndsFeedback?.PlayFeedbacks();
                return true;
            }

            Debug.Log("FuelInventory.RemoveItemByID: Item not removed");
            return false;
        }
    }
}
