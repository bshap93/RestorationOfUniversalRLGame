using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Prefabs.UI.PrefabRequiredScripts;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.InteractiveEntities;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.UI.HUD
{
    public class PreviewManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        
        [Header("UI References")]
        [SerializeField] private TMPInventoryDetails itemDetails; // For backward compatibility
        [SerializeField] private CraftingStationDetails craftingStationDetails;

        
        private IPreviewable currentPreviewable;
        private IDetailsDisplay currentDisplay;

        public InventoryItem CurrentPreviewedItem { get; set; }
        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens)
            {
                Debug.Log("Inventory Opens");
                HidePreview();
            }

            if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            {
                Debug.Log("Inventory Closes");
                HidePreview();
            }
        }


        public void ShowPreview(IPreviewable previewable)
        {
            
            if (previewable == null)
            {
                return;
            }

            // Hide current preview if different
            if (currentPreviewable != previewable)
            {
                HidePreview();
            }

            // Get appropriate display for the previewable type
            IDetailsDisplay display = GetDisplayForPreviewable(previewable);
            if (display == null) return;
            
            // Show the new preview
            display.DisplayDetails(previewable);
            ShowCanvasGroup(display.CanvasGroup);
            
                        
            currentPreviewable = previewable;
            currentDisplay = display;
            
            
            
            
            // if (InventoryDetails != null)
            // {
            //     InventoryDetails.DisplayPreview(item);
            //
            //     CurrentPreviewedItem = item;
            //
            //     // Make sure CanvasGroup is visible
            //     var canvasGroup = InventoryDetails.GetComponent<CanvasGroup>();
            //     if (canvasGroup != null)
            //     {
            //         canvasGroup.alpha = 1;
            //         canvasGroup.interactable = true;
            //         canvasGroup.blocksRaycasts = true;
            //     }
            // }
        }


        public void HidePreview()
        {
            if (currentDisplay != null)
            {
                currentDisplay.Hide();
                HideCanvasGroup(currentDisplay.CanvasGroup);
            }
            
            currentPreviewable = null;
            currentDisplay = null;
        }
        
        private IDetailsDisplay GetDisplayForPreviewable(IPreviewable previewable)
        {
            // For backward compatibility with existing InventoryItem system
            if (previewable is InventoryItem)
            {
                return itemDetails;
            }
            
            if (previewable is CraftingStationBehaviour)
            {
                return craftingStationDetails;
            }

            return null;
        }
        
        private void ShowCanvasGroup(CanvasGroup canvasGroup)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        private void HideCanvasGroup(CanvasGroup canvasGroup)
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }

    }
}
