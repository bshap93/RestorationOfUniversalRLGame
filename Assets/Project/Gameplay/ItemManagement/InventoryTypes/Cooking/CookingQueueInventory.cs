using System.Collections;
using MoreMountains.Feedbacks;
using Project.Gameplay.Interactivity.Food;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryTypes.Cooking
{
    public class CookingQueueInventory : CraftingQueueInventory
    {
        public MMFeedbacks addedRawFoodFeedback;
        public MMFeedbacks cookingStartsFeedback;
        public MMFeedbacks cookingEndsFeedback;

        public FuelInventory fuelInventory;
        public CookingDepositInventory cookingDepositInventory;


        public override bool AddItem(InventoryItem item, int quantity)
        {
            if (item is RawFood)
            {
                addedRawFoodFeedback?.PlayFeedbacks();
                var rawFoodItem = new RawFoodItem(item);
                StartCoroutine(CookFood(rawFoodItem, quantity));
                return base.AddItem(item, quantity);
            }

            return false;
        }

        public override bool AddItemAt(InventoryItem item, int quantity, int index)
        {
            if (item is RawFood)
            {
                addedRawFoodFeedback?.PlayFeedbacks();
                return base.AddItemAt(item, quantity, index);
            }

            return false;
        }

        IEnumerator CookFood(RawFoodItem rawFoodItem, int quantity)
        {
            if (!fuelInventory.IsBurning) yield break;
            float elapsedTime = 0;
            cookingStartsFeedback?.PlayFeedbacks();

            while (elapsedTime < rawFoodItem.cookDuration)
            {
                yield return null;

                elapsedTime += Time.deltaTime;
            }

            RemoveItem(0, quantity);
            cookingEndsFeedback?.PlayFeedbacks();
        }
    }
}
