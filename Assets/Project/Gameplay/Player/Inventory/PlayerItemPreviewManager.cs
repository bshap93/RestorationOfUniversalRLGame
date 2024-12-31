using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Events;
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
        readonly object _pickupLock = new();

        // Add to PlayerItemPreviewManager.cs
        string _currentPickupId;

        HighlightManager _highlightManager;

        bool _isPickingUp;
        bool _isPickupLocked;


        bool _isSorting;
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
            var itemPicker = eventType.ItemTransform.GetComponent<ManualItemPicker>();
            if (itemPicker == null) return;

            switch (eventType.EventName)
            {
                case "ItemPickupRangeEntered":
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
            if (_isPickupLocked)
            {
                Debug.Log($"Pickup check while locked - Current: {_currentPickupId}, Checking: {itemPicker.UniqueID}");
                return false;
            }

            var isPreviewed = CurrentPreviewedItemPicker != null &&
                              CurrentPreviewedItemPicker.UniqueID == itemPicker.UniqueID;

            Debug.Log($"Preview check for {itemPicker.UniqueID}: {isPreviewed}");
            return isPreviewed;
        }

        public bool TryPickupItem(ManualItemPicker itemPicker)
        {
            lock (_pickupLock)
            {
                if (_isPickingUp || CurrentPreviewedItemPicker != itemPicker) return false;

                _isPickingUp = true;
                _itemPickersInRange.Remove(itemPicker.UniqueID);
                UpdateNearestItem();
                _isPickingUp = false;
                return true;
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
                _itemPickersInRange.Remove(itemPicker.UniqueID);
                _highlightManager.UnselectObject(itemTransform);

                // Clear current preview if it was this item
                if (CurrentPreviewedItemPicker?.UniqueID == itemPicker.UniqueID)
                {
                    CurrentPreviewedItemPicker = null;
                    CurrentPreviewedItem = null;
                }

                if (_itemPickersInRange.Count == 0)
                    HidePreviewPanel();
                else
                    UpdateNearestItem();
            }
        }

        void UpdateNearestItem()
        {
            if (_isPickingUp)
            {
                Debug.Log("Skipping nearest item update - pickup in progress");
                return;
            }

            // Remove any null entries
            var invalidKeys = _itemPickersInRange
                .Where(kvp => kvp.Value == null || kvp.Value.gameObject == null)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var key in invalidKeys) _itemPickersInRange.Remove(key);

            if (_itemPickersInRange.Count == 0)
            {
                if (CurrentPreviewedItemPicker != null)
                {
                    _previewManager.HidePreview();
                    CurrentPreviewedItem = null;
                    CurrentPreviewedItemPicker = null;
                }

                return;
            }

            // Find the closest item picker
            var closestPicker = _itemPickersInRange.Values
                .OrderBy(picker => Vector3.Distance(transform.position, picker.transform.position))
                .FirstOrDefault();

            if (closestPicker != null && (CurrentPreviewedItemPicker == null ||
                                          closestPicker.UniqueID != CurrentPreviewedItemPicker.UniqueID))
            {
                SetPreviewedItem(closestPicker);
                _previewManager.ShowPreview(CurrentPreviewedItem);
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

        // void DisplayNearestItem()
        // {
        //     if (_isSorting) return;
        //
        //     _isSorting = true;
        //
        //     var destroyedKeys = new List<int>();
        //     foreach (var key in _itemTransforms.Keys.ToList())
        //         if (_itemTransforms[key] == null)
        //             destroyedKeys.Add(key);
        //
        //     foreach (var key in destroyedKeys) _itemTransforms.Remove(key);
        //
        //     // Remove null or destroyed items from the list
        //     _itemsInRange.RemoveAll(item => item == null || !_itemTransforms.ContainsKey(item.GetInstanceID()));
        //
        //     if (_itemsInRange.Count == 0 || _previewManager == null)
        //     {
        //         if (CurrentPreviewedItem != null)
        //         {
        //             _previewManager.HidePreview();
        //             CurrentPreviewedItem = null;
        //             CurrentPreviewedItemPicker = null;
        //         }
        //
        //         _isSorting = false;
        //         return;
        //     }
        //
        //     // Sort to get the closest item
        //     _itemsInRange.Sort(
        //         (a, b) =>
        //         {
        //             if (!_itemTransforms.ContainsKey(a.GetInstanceID()) ||
        //                 !_itemTransforms.ContainsKey(b.GetInstanceID()))
        //                 return 0; // Skip if either item transform is missing
        //
        //             var transformA = _itemTransforms[a.GetInstanceID()];
        //             var transformB = _itemTransforms[b.GetInstanceID()];
        //
        //             return Vector3.Distance(transform.position, transformA.position)
        //                 .CompareTo(Vector3.Distance(transform.position, transformB.position));
        //         });
        //
        //     var closestItem = _itemTransforms[_itemsInRange[0].GetInstanceID()].GetComponent<ManualItemPicker>();
        //     if (closestItem != null && CurrentPreviewedItemPicker != closestItem)
        //     {
        //         SetPreviewedItem(closestItem); // Now sets the actual `ManualItemPicker`
        //         _previewManager.ShowPreview(CurrentPreviewedItem);
        //     }
        //
        //     _isSorting = false;
        // }
        //
        //
        // public void RegisterItem(InventoryItem item)
        // {
        //     if (!_itemsInRange.Contains(item)) _itemsInRange.Add(item);
        //     Debug.Log("Item registered");
        // }

        // public void UnregisterItem(InventoryItem item)
        // {
        //     if (_itemsInRange.Contains(item))
        //     {
        //         _itemsInRange.Remove(item);
        //
        //         // Reset current item if it was removed
        //         if (CurrentPreviewedItem == item)
        //         {
        //             _previewManager.HidePreview();
        //             CurrentPreviewedItem = null;
        //         }
        //     }
        // }
    }
}
