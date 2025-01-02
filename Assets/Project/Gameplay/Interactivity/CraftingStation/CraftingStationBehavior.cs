using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Interactivity.InteractiveEntities;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class CraftingStationBehaviour : MonoBehaviour, IInteractable, IPreviewable
    {
        [SerializeField] private CraftingStation craftingStationData;
        public CraftingStation CraftingStationData => craftingStationData; // Allow read-only access
    
        private string _currentPlayerId;
    
        // IInteractable Implementation remains the same
        public InteractionType GetInteractionType() => InteractionType.CraftingStation;
    
        public bool CanInteract(Character playerCharacter)
        {
            if (playerCharacter == null) return false;
        
            _currentPlayerId = playerCharacter.PlayerID;
            var sourceInv = craftingStationData.SourceInventory(_currentPlayerId);
            var targetInv = craftingStationData.TargetInventory(_currentPlayerId);
        
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
    
        public Transform GetTransform() => transform;
    
        // IPreviewable Implementation
        public void ShowPreview()
        {
            // Use the same event system as InventoryItem
            MMEventManager.TriggerEvent(new PreviewEvent
            {
                EventType = PreviewEventType.ShowPreview,
                Previewable = this
            });
        }
    
        public void HidePreview()
        {
            MMEventManager.TriggerEvent(new PreviewEvent
            {
                EventType = PreviewEventType.HidePreview,
                Previewable = this
            });
        }
    
        public PreviewType GetPreviewType() => PreviewType.CraftingStation;
    
        public float GetPreviewPriority() => 2.0f; // Higher than items
    
        // Station-specific handling methods
        private void HandleCookingInteraction(Character playerCharacter)
        {
            var sourceInv = craftingStationData.SourceInventory(_currentPlayerId);
            var targetInv = craftingStationData.TargetInventory(_currentPlayerId);
            var queueInv = craftingStationData.QueueInventory(_currentPlayerId);
        
            // TODO: Implement cooking logic
            // - Check for required ingredients in source inventory
            // - Move ingredients to queue if appropriate
            // - Start cooking process
            // - Handle completion and move to target inventory
        }
    
        private void HandleForgingInteraction(Character playerCharacter)
        {
            // Similar structure to cooking
            // TODO: Implement forging-specific logic
        }
    
        private void HandleItemRemovalInteraction(Character playerCharacter)
        {
            // TODO: Implement item removal logic
        }
    
        // Optional: Add methods for the cooking process
        private void StartCooking()
        {
            craftingStationData.CraftingInProgressFeedbacks?.PlayFeedbacks();
        }
    
        private void CompleteCooking()
        {
            craftingStationData.CraftingCompleteFeedbacks?.PlayFeedbacks();
        }
    }
}