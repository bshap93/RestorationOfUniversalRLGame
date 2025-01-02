using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using Project.UI.HUD;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation.Cooking
{
    public class CookingStationTest : CraftingStationBehaviour
    {
        [Header("Cooking Settings")] [SerializeField]
        string fuelItemID = "Firewood01";
        [SerializeField] float cookingTime = 3f;

        [Header("Test Recipe")] [SerializeField]
        string rawFoodID = "Carrots";
        [SerializeField] InventoryConsumable cookedFoodPrefab; // Reference CarrotStew ScriptableObject
        float cookingTimer;

        bool isCooking;

        void Update()
        {
            if (isCooking)
            {
                cookingTimer += Time.deltaTime;
                if (cookingTimer >= cookingTime) CompleteCooking();
            }
        }

        void HandleCookingInteraction(Character playerCharacter)
        {
            if (isCooking) return;

            var sourceInv = CraftingStationData.SourceInventory(CurrentPlayerId);
            if (sourceInv == null) return;

            // Check for ingredients
            var foodSlots = sourceInv.InventoryContains(rawFoodID);

            if (foodSlots.Count > 0)
            {
                // Check for fuel
                var fuelSlots = sourceInv.InventoryContains(fuelItemID);

                if (fuelSlots.Count > 0)
                {
                    // Remove one fuel
                    sourceInv.RemoveItem(fuelSlots[0], 1);

                    // Remove one raw food
                    sourceInv.RemoveItem(foodSlots[0], 1);

                    // Start cooking
                    StartCooking();

                    // Play feedback
                    CraftingStationData.CraftingInProgressFeedbacks?.PlayFeedbacks();
                }
            }
        }

        void StartCooking()
        {
            isCooking = true;
            cookingTimer = 0f;

            // Update UI if it's being previewed
            MMEventManager.TriggerEvent(
                new PreviewEvent
                {
                    EventType = PreviewEventType.ShowPreview,
                    Previewable = this
                });
        }

        void CompleteCooking()
        {
            isCooking = false;
            cookingTimer = 0f;

            var targetInv = CraftingStationData.TargetInventory(CurrentPlayerId);

            // Add the cooked food item
            if (cookedFoodPrefab != null) targetInv.AddItem(cookedFoodPrefab, 1);

            // Play completion feedback
            CraftingStationData.CraftingCompleteFeedbacks?.PlayFeedbacks();

            // Update UI if it's being previewed
            MMEventManager.TriggerEvent(
                new PreviewEvent
                {
                    EventType = PreviewEventType.ShowPreview,
                    Previewable = this
                });
        }
    }
}
