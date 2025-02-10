using System;
using Gameplay.Extensions.InventoryEngineExtensions.PickupDisplayer;
using Gameplay.Player.Inventory;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.UI.HUD
{
    public class ListPreviewManager : MonoBehaviour, MMEventListener<MMInventoryEvent>

    {
        public PickableItemsListPanel PickableItemsListPanel;
        [FormerlySerializedAs("CraftingStationDetails")]
        public DispenserItemPanel DispenserItemPanel;


        public CraftingStation CurrentPreviewedCraftingStation { get; set; }

        void Start()
        {
            HideItemListPreview();
            HideDispenserItemPreview();
        }
        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
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


        public void AddToItemListPreview(InventoryItem currentPreviewedItem, ManualItemPicker manualItemPicker)
        {
            PickableItemsListPanel.AddItemToItemsList(currentPreviewedItem, manualItemPicker);
        }

        public void RemoveFromItemListPreview(InventoryItem currentPreviewedItem)
        {
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
        public bool IsItemInList(BaseItem itemPickerItem)
        {
            throw new NotImplementedException();
        }

        public void ShowDispenserItemPreview(DispenserItem dispenser)
        {
            if (DispenserItemPanel != null)
            {
                DispenserItemPanel.SetItem(
                    dispenser.DispensedItem,
                    dispenser.TotalSupply,
                    dispenser.TotalSupply, // Assuming the cap is the initial stock
                    "Dispenser"
                );

                // Make sure it's visible
                var canvasGroup = DispenserItemPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void HideDispenserItemPreview()
        {
            if (DispenserItemPanel != null)
            {
                var canvasGroup = DispenserItemPanel.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
    }
}
