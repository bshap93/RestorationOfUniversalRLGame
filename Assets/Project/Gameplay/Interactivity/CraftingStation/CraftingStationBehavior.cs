using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Interactivity.InteractiveEntities;
using Project.UI.HUD;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class CraftingStationBehaviour : MonoBehaviour, IInteractable, IPreviewable
    {
        [SerializeField] CraftingStation craftingStationData;

        protected string CurrentPlayerId;
        public CraftingStation CraftingStationData => craftingStationData; // Allow read-only access

        // IInteractable Implementation remains the same
        public InteractionType GetInteractionType()
        {
            return InteractionType.CraftingStation;
        }

        public bool CanInteract(Character playerCharacter)
        {
            if (playerCharacter == null) return false;

            CurrentPlayerId = playerCharacter.PlayerID;
            var sourceInv = craftingStationData.SourceInventory(CurrentPlayerId);
            var targetInv = craftingStationData.TargetInventory(CurrentPlayerId);

            // Basic validation
            return sourceInv != null && targetInv != null;
        }

        public void OnInteract(Character playerCharacter)
        {
            if (!CanInteract(playerCharacter)) return;

            craftingStationData.Interact();

            switch (craftingStationData.StationType)
            {
                case CraftingStationType.CookingStation:
                    HandleCookingInteraction(playerCharacter);
                    break;
                case CraftingStationType.ForgingStation:
                    HandleForgingInteraction(playerCharacter);
                    break;
                case CraftingStationType.ItemRemovalStation:
                    HandleItemRemovalInteraction(playerCharacter);
                    break;
            }
        }

        public Transform GetTransform()
        {
            return transform;
        }

        // IPreviewable Implementation
        public void ShowPreview()
        {
            // Use the same event system as InventoryItem
            MMEventManager.TriggerEvent(
                new PreviewEvent
                {
                    EventType = PreviewEventType.ShowPreview,
                    Previewable = this
                });
        }

        public void HidePreview()
        {
            MMEventManager.TriggerEvent(
                new PreviewEvent
                {
                    EventType = PreviewEventType.HidePreview,
                    Previewable = this
                });
        }

        public PreviewType GetPreviewType()
        {
            return PreviewType.CraftingStation;
        }

        public float GetPreviewPriority()
        {
            return 2.0f;
            // Higher than items
        }

        // Station-specific handling methods
        void HandleCookingInteraction(Character playerCharacter)
        {
            var sourceInv = craftingStationData.SourceInventory(CurrentPlayerId);
            var targetInv = craftingStationData.TargetInventory(CurrentPlayerId);
            var queueInv = craftingStationData.QueueInventory(CurrentPlayerId);

            // TODO: Implement cooking logic
            // - Check for required ingredients in source inventory
            // - Move ingredients to queue if appropriate
            // - Start cooking process
            // - Handle completion and move to target inventory
        }

        void HandleForgingInteraction(Character playerCharacter)
        {
            // Similar structure to cooking
            // TODO: Implement forging-specific logic
        }

        void HandleItemRemovalInteraction(Character playerCharacter)
        {
            // TODO: Implement item removal logic
        }

        // Optional: Add methods for the cooking process
        void StartCooking()
        {
            craftingStationData.CraftingInProgressFeedbacks?.PlayFeedbacks();
        }

        void CompleteCooking()
        {
            craftingStationData.CraftingCompleteFeedbacks?.PlayFeedbacks();
        }
    }
}
