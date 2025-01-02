using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.Events;
using Project.Gameplay.Interactivity.Items;
using Project.UI.HUD;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class PlayerItemPreviewManager : MonoBehaviour, MMEventListener<ItemEvent>
    {
        public GameObject PreviewPanelUI;

        public MMFeedbacks SelectionFeedbacks;
        public MMFeedbacks DeselectionFeedbacks;
        readonly Dictionary<string, ManualItemPicker> _itemPickersInRange = new();
        readonly float _pickupCooldown = 0.5f; // Add cooldown to prevent rapid pickups
        readonly object _pickupLock = new();

        // Add to PlayerItemPreviewManager.cs
        string _currentPickupId;

        HighlightManager _highlightManager;
        bool _isPickingUp;

        bool _isPickupLocked;


        bool _isSorting;
        float _lastPickupTime;
        PreviewManager _previewManager;
        public ManualItemPicker CurrentPreviewedItemPicker { get; private set; }
        public InventoryItem CurrentPreviewedItem { get; private set; }


        void Start()
        {
            _previewManager = FindObjectOfType<PreviewManager>();
            _highlightManager = FindObjectOfType<HighlightManager>();

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


        public void OnMMEvent(ItemEvent eventType)
        {
            // Skip if we're in the middle of a pickup
            if (_isPickingUp) return;

            var itemPicker = eventType.ItemTransform.GetComponent<ManualItemPicker>();
            if (itemPicker == null) return;

            switch (eventType.EventName)
            {
                case "ItemPickupRangeEntered":
                    if (!_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
                        HandleItemEntered(itemPicker, eventType.ItemTransform);

                    break;

                case "ItemPickupRangeExited":
                case "ItemPickedUp":
                    HandleItemExited(itemPicker, eventType.ItemTransform);
                    break;
            }
        }

        public void LockPickup(string pickupId)
        {
            Debug.Log($"Locking pickup for item {pickupId}");
            _currentPickupId = pickupId;
            _isPickupLocked = true;
        }

        public void UnlockPickup()
        {
            Debug.Log($"Unlocking pickup (was locked for {_currentPickupId})");
            _currentPickupId = null;
            _isPickupLocked = false;
        }

        public bool IsPreviewedItem(ManualItemPicker itemPicker)
        {
            if (_isPickupLocked) return false;

            var isPreviewed = CurrentPreviewedItemPicker != null &&
                              CurrentPreviewedItemPicker.UniqueID == itemPicker.UniqueID;

            return isPreviewed;
        }

        public bool TryPickupItem(ManualItemPicker itemPicker)
        {
            if (_isPickingUp || Time.time - _lastPickupTime < _pickupCooldown) return false;

            if (CurrentPreviewedItemPicker?.UniqueID != itemPicker.UniqueID) return false;

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
                        CurrentPreviewedItemPicker = null;
                        CurrentPreviewedItem = null;
                        _previewManager.HidePreview();
                        HidePreviewPanel();
                    }
                    else
                    {
                        // Clear current preview and let UpdateNearestItem handle the next item
                        CurrentPreviewedItemPicker = null;
                        CurrentPreviewedItem = null;
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
            if (!_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
            {
                _itemPickersInRange.Add(itemPicker.UniqueID, itemPicker);
                _highlightManager.SelectObject(itemTransform);

                // Only show preview panel if this is the first/only item
                if (_itemPickersInRange.Count == 1) ShowPreviewPanel(itemPicker.Item);

                // Force update of nearest item
                UpdateNearestItem();
            }
        }

        void HandleItemExited(ManualItemPicker itemPicker, Transform itemTransform)
        {
            if (_itemPickersInRange.ContainsKey(itemPicker.UniqueID))
            {
                var wasCurrentlyPreviewed = CurrentPreviewedItemPicker?.UniqueID == itemPicker.UniqueID;
                _itemPickersInRange.Remove(itemPicker.UniqueID);
                _highlightManager.UnselectObject(itemTransform);

                // Only if this was the last item AND it was being previewed, reset everything
                if (_itemPickersInRange.Count == 0 && wasCurrentlyPreviewed)
                {
                    CurrentPreviewedItemPicker = null;
                    CurrentPreviewedItem = null;
                    HidePreviewPanel();
                    _previewManager.HidePreview();
                }
                // If items remain and we removed the previewed item, update to show the next one
                else if (wasCurrentlyPreviewed)
                {
                    UpdateNearestItem();
                }
            }
        }

        void UpdateNearestItem()
        {
            if (_isPickingUp)
            {
                Debug.Log("Skipping nearest item update - pickup in progress");
                return;
            }

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
                    if (CurrentPreviewedItemPicker != null)
                    {
                        CurrentPreviewedItemPicker = null;
                        CurrentPreviewedItem = null;
                        _previewManager.HidePreview();
                        HidePreviewPanel();
                    }

                    return;
                }

                var closestPicker = _itemPickersInRange.Values
                    .Where(picker => picker != null && picker.gameObject != null)
                    .OrderBy(picker => Vector3.Distance(transform.position, picker.transform.position))
                    .FirstOrDefault();

                if (closestPicker != null && (CurrentPreviewedItemPicker == null ||
                                              closestPicker.UniqueID != CurrentPreviewedItemPicker.UniqueID))
                {
                    SetPreviewedItem(closestPicker);
                    _previewManager.ShowPreview(CurrentPreviewedItem);
                }
            }
            finally
            {
                _isSorting = false;
            }
        }


        public void SetPreviewedItem(ManualItemPicker itemPicker)
        {
            CurrentPreviewedItem = itemPicker.Item;
            CurrentPreviewedItemPicker = itemPicker;
        }


        public void HidePreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
        }

        public void ShowPreviewPanel(InventoryItem item)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
        }


        public void ShowSelectedItemPreviewPanel(InventoryItem item)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
            _previewManager.ShowPreview(item);
        }

        public void HideSelectedItemPreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
            _previewManager.HidePreview();
        }
    }
}
