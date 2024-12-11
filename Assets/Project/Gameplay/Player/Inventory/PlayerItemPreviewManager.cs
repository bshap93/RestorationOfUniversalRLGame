﻿using System.Collections.Generic;
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
        readonly List<InventoryItem> _itemsInRange = new(); // List of items in range
        readonly Dictionary<string, Transform> _itemTransforms = new(); // Dictionary of item transforms

        bool _isSorting;
        PreviewManager _previewManager;
        public InventoryItem CurrentPreviewedItem { get; private set; }


        void Start()
        {
            _previewManager = FindObjectOfType<PreviewManager>();
            if (_previewManager == null) Debug.LogWarning("PreviewManager not found in the scene.");
        }

        void Update()
        {
            DisplayNearestItem();
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
            if (eventType.EventName == "ItemPickupRangeEntered")
            {
                _itemsInRange.Add(eventType.Item);
                _itemTransforms.Add(eventType.Item.ItemID, eventType.ItemTransform);


                ShowPreviewPanel(eventType.Item);
            }

            if (eventType.EventName == "ItemPickupRangeExited" || eventType.EventName == "ItemPickedUp")
            {
                if (_itemsInRange.Contains(eventType.Item)) _itemsInRange.Remove(eventType.Item);

                if (_itemTransforms.ContainsKey(eventType.Item.ItemID)) _itemTransforms.Remove(eventType.Item.ItemID);

                if (_itemsInRange.Count == 0)
                    HidePreviewPanel();
                else
                    ShowPreviewPanel(_itemsInRange[0]);
            }
        }

        public void HidePreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
        }

        public void ShowPreviewPanel(InventoryItem item)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
        }

        void DisplayNearestItem()
        {
            // Avoid updating the preview if sorting is currently happening
            if (_isSorting) return;

            _isSorting = true;

            // Remove items that no longer exist in _itemTransforms (clean up destroyed items)
            var destroyedKeys = new List<string>();
            foreach (var entry in _itemTransforms)
                if (entry.Value == null)
                    destroyedKeys.Add(entry.Key);

            foreach (var key in destroyedKeys) _itemTransforms.Remove(key);

            // Remove null or destroyed items from the list
            _itemsInRange.RemoveAll(item => item == null || !_itemTransforms.ContainsKey(item.ItemID));

            if (_itemsInRange.Count == 0 || _previewManager == null)
            {
                if (CurrentPreviewedItem != null)
                {
                    _previewManager.HidePreview();
                    CurrentPreviewedItem = null;
                }

                _isSorting = false;
                return;
            }

            // Sort to get the closest item
            _itemsInRange.Sort(
                (a, b) =>
                {
                    if (!_itemTransforms.ContainsKey(a.ItemID) || !_itemTransforms.ContainsKey(b.ItemID))
                    {
                        Debug.LogError($"Missing key in _itemTransforms for ItemID a: {a.ItemID} or b: {b.ItemID}");
                        return 0;
                    }

                    var transformA = _itemTransforms[a.ItemID];
                    var transformB = _itemTransforms[b.ItemID];

                    // Check if the transforms are null or destroyed
                    if (transformA == null || transformB == null)
                    {
                        Debug.LogError($"Transform for ItemID a: {a.ItemID} or b: {b.ItemID} is destroyed");
                        return 0;
                    }

                    return Vector3.Distance(transform.position, transformA.position)
                        .CompareTo(Vector3.Distance(transform.position, transformB.position));
                }
            );

            var closestItem = _itemsInRange[0];
            if (CurrentPreviewedItem == null || CurrentPreviewedItem != closestItem)
            {
                CurrentPreviewedItem = _itemsInRange[0];
                _previewManager.ShowPreview(CurrentPreviewedItem);
            }

            _isSorting = false;
        }


        public void RegisterItem(InventoryItem item)
        {
            if (!_itemsInRange.Contains(item)) _itemsInRange.Add(item);
        }

        public void UnregisterItem(InventoryItem item)
        {
            if (_itemsInRange.Contains(item))
            {
                _itemsInRange.Remove(item);

                // Reset current item if it was removed
                if (CurrentPreviewedItem == item)
                {
                    _previewManager.HidePreview();
                    CurrentPreviewedItem = null;
                }
            }
        }
    }
}
