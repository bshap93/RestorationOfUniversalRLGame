using Gameplay.Player.Inventory;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Prefabs.UI.PrefabRequiredScripts;
using Project.Core.Events;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.UI.HUD
{
    public class ListPreviewManager : MonoBehaviour, MMEventListener<MMInventoryEvent>,
        MMEventListener<CookingStationEvent>
    {
        public PickableItemsListPanel PickableItemsListPanel;
        [FormerlySerializedAs("CraftingStationDetails")]
        public TMPCookingStationDetails CookingStationDetails;


        public CraftingStation CurrentPreviewedCraftingStation { get; set; }

        void Start()
        {
            HideItemListPreview();
        }
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
                ShowCookingStationPreview(mmEvent.CookingStationControllerParameter.CookingStation);
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


        public void ShowCookingStationPreview(CookingStation craftingStation)
        {
            if (CookingStationDetails != null)
            {
                CookingStationDetails.DisplayPreview(craftingStation);

                CurrentPreviewedCraftingStation = craftingStation;


                // Make sure CanvasGroup is visible
                var canvasGroup = CookingStationDetails.GetComponent<CanvasGroup>();
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
            if (CookingStationDetails != null)
            {
                var canvasGroup = CookingStationDetails.GetComponent<CanvasGroup>();
                CurrentPreviewedCraftingStation = null;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
        public void AddToItemListPreview(InventoryItem currentPreviewedItem, ManualItemPicker manualItemPicker)
        {
            PickableItemsListPanel.AddItemToItemsList(currentPreviewedItem, manualItemPicker);
        }

        public void RemoveFromItemListPreview(InventoryItem currentPreviewedItem)
        {
            Debug.Log("Removing: " + currentPreviewedItem.name);
            PickableItemsListPanel.RemoveItemFromItemsList(currentPreviewedItem);
        }
        public void RemoveAllFromItemList()
        {
            PickableItemsListPanel.RemoveAllItemsFromList();
        }
        public void HidePanelIfEmpty()
        {
            if (PickableItemsListPanel.CurrentPreviewedItems.Count == 0) HideItemListPreview();
        }
        public void RefreshPreviewOrder()
        {
            PickableItemsListPanel.RefreshPreviewOrder();
        }
    }
}
