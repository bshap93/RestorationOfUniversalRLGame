using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using Project.Gameplay.Events;
using Project.Gameplay.Interactivity.Items;
using Project.UI.HUD;
using UnityEngine;

namespace Gameplay.Player.Inventory
{
    public class PlayerItemListPreviewManager : MonoBehaviour, MMEventListener<ItemEvent>
    {
        public GameObject PreviewPanelUI;

        public MMFeedbacks SelectionFeedbacks;
        public MMFeedbacks DeselectionFeedbacks;
        public List<ManualItemPicker> CurrentPreviewedItemPickers = new();
        readonly Dictionary<string, ManualItemPicker> _itemPickersInRange = new();
        readonly float _pickupCooldown = 0.5f; // Add cooldown to prevent rapid pickups

        // Add to PlayerItemPreviewManager.cs

        HighlightManager _highlightManager;
        bool _isPickingUp;

        bool _isPickupLocked;


        bool _isSorting;
        float _lastPickupTime;
        ListPreviewManager _previewManager;
        public List<InventoryItem> CurrentPreviewedItems { get; private set; }


        void Awake()
        {
            _previewManager = FindFirstObjectByType<ListPreviewManager>();
            _highlightManager = FindFirstObjectByType<HighlightManager>();

            if (_previewManager == null) Debug.LogWarning("PreviewManager not found in the scene.");
            if (_highlightManager == null) Debug.LogWarning("HighlightManager not found in the scene.");
        }

        void Update()
        {
            if (!_isSorting) UpdateNearestItem();
        }

        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening();
        }


        public void OnMMEvent(ItemEvent mmEvent)
        {
            // Skip if we're in the middle of a pickup
            if (_isPickingUp) return;

            var itemPicker = mmEvent.ItemTransform.GetComponent<ManualItemPicker>();
            if (itemPicker == null) return;

            switch (mmEvent.EventName)
            {
                case "ItemPickupRangeEntered":
                    if (!_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
                        HandleItemEntered(itemPicker, mmEvent.ItemTransform);

                    break;

                case "ItemPickupRangeExited":
                case "ItemPickedUp":
                    HandleItemExited(itemPicker, mmEvent.ItemTransform);
                    break;
            }
        }

        public bool IsPreviewedItem(ManualItemPicker itemPicker)
        {
            if (_isPickupLocked) return false;

            var isPreviewed = CurrentPreviewedItemPickers != null && CurrentPreviewedItemPickers.Count != 0 &&
                              CurrentPreviewedItemPickers.Any(picker => picker.UniqueID == itemPicker.UniqueID);

            return isPreviewed;
        }

        public bool TryPickupItem(ManualItemPicker itemPicker)
        {
            if (_isPickingUp || Time.time - _lastPickupTime < _pickupCooldown) return false;

            if (itemPicker.Item.UsageType == ItemUsageType.Usable)
            {
                Debug.Log($"Cannot pick up {itemPicker.Item.ItemName} because it is not pickable.");
                return false;
            }

            if (_isPickingUp || Time.time - _lastPickupTime < _pickupCooldown) return false;

            if (CurrentPreviewedItemPickers == null || CurrentPreviewedItemPickers.Count == 0 ||
                !CurrentPreviewedItemPickers.Any(picker => picker.UniqueID == itemPicker.UniqueID)) return false;

            _isPickingUp = true;
            _lastPickupTime = Time.time;

            try
            {
                if (_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
                {
                    _itemPickersInRange.Remove(itemPicker.UniqueID);

                    // If this was the last item, clear everything
                    if (_itemPickersInRange.Count == 0)
                    {
                        _previewManager.RemoveFromItemListPreview(itemPicker.Item);
                        CurrentPreviewedItemPickers = new List<ManualItemPicker>();
                        CurrentPreviewedItems = new List<InventoryItem>();
                        _previewManager.HideItemListPreview();
                        HidePreviewPanel();
                    }
                    else
                    {
                        // Clear current preview and let UpdateNearestItem handle the next item
                        CurrentPreviewedItemPickers.Remove(itemPicker);
                        CurrentPreviewedItems.Remove(itemPicker.Item);
                        UpdateNearestItem();
                    }

                    return true;
                }

                return false;
            }
            finally
            {
                _isPickingUp = false;
            }
        }


        void HandleItemEntered(ManualItemPicker itemPicker, Transform itemTransform)
        {
            // Only add the item if it's not already tracked by its UniqueID
            if (_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
            {
                Debug.Log($"Item {itemPicker.UniqueID} is already in range. Skipping duplicate entry.");
                return;
            }

            _itemPickersInRange.Add(itemPicker.UniqueID, itemPicker);
            CurrentPreviewedItemPickers.Add(itemPicker);

            // Add the item to the preview list, ensuring it's tracked uniquely
            if (itemPicker.Item != null) _previewManager.AddToItemListPreview(itemPicker.Item, itemPicker);

            _highlightManager.SelectObject(itemTransform);

            // Show the preview panel if this is the first/only item
            if (_itemPickersInRange.Count == 1) ShowPreviewPanel();

            UpdateNearestItem(); // Ensure the nearest item is calculated
        }


        void HandleItemExited(ManualItemPicker itemPicker, Transform itemTransform)
        {
            if (!_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
            {
                return;
            }

            _itemPickersInRange.Remove(itemPicker.UniqueID);
            CurrentPreviewedItemPickers.Remove(itemPicker);

            // Remove the specific item instance from the preview
            if (itemPicker.Item != null) _previewManager.RemoveFromItemListPreview(itemPicker.Item);

            _highlightManager.UnselectObject(itemTransform);

            // Hide the preview panel if no items remain in range
            if (_itemPickersInRange.Count == 0)
            {
                HidePreviewPanel();
                _previewManager.HideItemListPreview();
            }
        }


        void UpdateNearestItem()
        {
            if (_isPickingUp) return;

            _isSorting = true;

            try
            {
                var invalidKeys = _itemPickersInRange
                    .Where(kvp => kvp.Value == null || kvp.Value.gameObject == null)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in invalidKeys)
                    _itemPickersInRange.Remove(key);

                if (_itemPickersInRange.Count == 0)
                {
                    if (CurrentPreviewedItemPickers != null)
                    {
                        _previewManager.RemoveAllFromItemList();
                        CurrentPreviewedItemPickers = new List<ManualItemPicker>();
                        CurrentPreviewedItems = new List<InventoryItem>();
                        _previewManager.HideItemListPreview();
                        HidePreviewPanel();
                    }

                    return;
                }

                var closestPicker = _itemPickersInRange.Values
                    .Where(picker => picker != null && picker.gameObject != null)
                    .OrderBy(picker => Vector3.Distance(transform.position, picker.transform.position))
                    .FirstOrDefault();

                if (CurrentPreviewedItemPickers == null || CurrentPreviewedItemPickers.Count == 0 ||
                    !CurrentPreviewedItemPickers.Any(
                        picker => closestPicker != null && picker.UniqueID == closestPicker.UniqueID))
                    // SetPreviewedItem(closestPicker);
                    // _previewManager.AddToItemListPreview(CurrentPreviewedItem);
                    _previewManager.ShowItemListPreview();
            }
            finally
            {
                _isSorting = false;
            }
        }

        public void AddToItemListPreview(InventoryItem item, ManualItemPicker manualItemPicker = null)
        {
            if (item == null || manualItemPicker == null) return;

            // Ensure no duplicates by checking the UniqueID
            if (CurrentPreviewedItemPickers.Any(picker => picker.UniqueID == manualItemPicker.UniqueID))
            {
                Debug.Log($"Item with UniqueID {manualItemPicker.UniqueID} is already in the preview list.");
                return;
            }

            _previewManager.AddToItemListPreview(item, manualItemPicker);
            CurrentPreviewedItemPickers.Add(manualItemPicker);
            RefreshPreviewOrder();
        }


        // public void SetPreviewedItem(ManualItemPicker itemPicker)
        // {
        //     CurrentPreviewedItems = itemPicker.Item;
        //     CurrentPreviewedItemPickers = itemPicker;
        // }


        public void HidePreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
        }

        public void ShowPreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
        }


        public void ShowSelectedItemPreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
            _previewManager.ShowItemListPreview();
        }

        public void HideSelectedItemPreviewPanel(InventoryItem item)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
            _previewManager.HideItemListPreview();
        }
        public void HidePanelIfEmpty(InventoryItem item)
        {
            _previewManager.HidePanelIfEmpty();
        }
        public void RemoveFromItemListPreview(InventoryItem item)
        {
            _previewManager.RemoveFromItemListPreview(item);
        }
        public void RefreshPreviewOrder()
        {
            _previewManager.RefreshPreviewOrder();
        }
    }
}
