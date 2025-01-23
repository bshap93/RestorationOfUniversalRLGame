using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Prefabs.UI.PrefabRequiredScripts;
using Project.Core.Events;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.UI.HUD
{
    public class ListPreviewManager : MonoBehaviour, MMEventListener<MMInventoryEvent>,
        MMEventListener<CookingStationEvent>
    {
        public PickableItemsListPanel PickableItemsListPanel;
        public TMPCraftingStationDetails CraftingStationDetails;


        public List<InventoryItem> CurrentPreviewedItems { get; set; }
        public CraftingStation CurrentPreviewedCraftingStation { get; set; }
        void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();
            this.MMEventStartListening<CookingStationEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();

            this.MMEventStopListening<CookingStationEvent>();
        }
        public void OnMMEvent(CookingStationEvent mmEvent)
        {
            if (mmEvent.EventType == CookingStationEventType.CookingStationSelected) HideCraftingStationPreviw();

            if (mmEvent.EventType == CookingStationEventType.CookingStationInRange)
                ShowCraftingStationPreview(mmEvent.CookingStationControllerParameter.CookingStation);
        }

        public void OnMMEvent(MMInventoryEvent mmEvent)
        {
            if (mmEvent.InventoryEventType == MMInventoryEventType.InventoryOpens) HideItemListPreview();

            if (mmEvent.InventoryEventType == MMInventoryEventType.InventoryCloses) HideItemListPreview();
        }


        public void ShowItemListPreview()
        {
            if (PickableItemsListPanel != null)
            {
                PickableItemsListPanel.DisplayPreview();


                // Make sure CanvasGroup is visible
                var canvasGroup = PickableItemsListPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void HideItemListPreview()
        {
            if (PickableItemsListPanel != null)
            {
                var canvasGroup = PickableItemsListPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }


        public void ShowCraftingStationPreview(CraftingStation craftingStation)
        {
            if (CraftingStationDetails != null)
            {
                CraftingStationDetails.DisplayPreview(craftingStation);

                CurrentPreviewedCraftingStation = craftingStation;


                // Make sure CanvasGroup is visible
                var canvasGroup = CraftingStationDetails.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }
        public void HideCraftingStationPreviw()
        {
            if (CraftingStationDetails != null)
            {
                var canvasGroup = CraftingStationDetails.GetComponent<CanvasGroup>();
                CurrentPreviewedCraftingStation = null;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
        public void AddToItemListPreview(InventoryItem currentPreviewedItem)
        {
            Debug.Log("Adding: " + currentPreviewedItem.name);
            PickableItemsListPanel.AddItemToItemsList(currentPreviewedItem);
        }

        public void RemoveFromItemListPreview(InventoryItem currentPreviewedItem)
        {
            Debug.Log("Removing: " + currentPreviewedItem.name);
        }
    }
}
