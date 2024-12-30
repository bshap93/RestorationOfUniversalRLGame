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
        readonly List<InventoryItem> _itemsInRange = new(); // List of items in range
        readonly Dictionary<int, Transform> _itemTransforms = new(); // Dictionary of item transforms
        HighlightManager _highlightManager;


        bool _isSorting;
        PreviewManager _previewManager;
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


                if (!_itemTransforms.ContainsKey(eventType.Item.GetInstanceID()))
                    _itemTransforms.Add(eventType.Item.GetInstanceID(), eventType.ItemTransform);
                else
                    Debug.LogWarning($"Item with ID {eventType.Item.GetInstanceID()} is already in _itemTransforms.");


                ShowPreviewPanel(eventType.Item);

                _highlightManager.SelectObject(eventType.ItemTransform);
            }

            if (eventType.EventName == "ItemPickupRangeExited" || eventType.EventName == "ItemPickedUp")
            {
                if (_itemsInRange.Contains(eventType.Item)) _itemsInRange.Remove(eventType.Item);

                if (_itemTransforms.ContainsKey(eventType.Item.GetInstanceID()))
                    _itemTransforms.Remove(eventType.Item.GetInstanceID());

                if (_itemsInRange.Count == 0)
                    HidePreviewPanel();
                else
                    ShowPreviewPanel(_itemsInRange[0]);

                _highlightManager.UnselectObject(eventType.ItemTransform);
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

        public void ShowSelectedItemPreviewPanel(InventoryItem item)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);

            Debug.Log("Item: " + item.name + " selected!");

            _previewManager.ShowPreview(item);
        }

        void DisplayNearestItem()
        {
            if (_isSorting) return;

            _isSorting = true;

            var destroyedKeys = new List<int>();

            foreach (var key in _itemTransforms.Keys.ToList())
                if (_itemTransforms[key] == null)
                    _itemTransforms.Remove(key);


            foreach (var key in destroyedKeys) _itemTransforms.Remove(key);

            // Remove null or destroyed items from the list
            _itemsInRange.RemoveAll(item => item == null || !_itemTransforms.ContainsKey(item.GetInstanceID()));

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
                    if (!_itemTransforms.ContainsKey(a.GetInstanceID()) ||
                        !_itemTransforms.ContainsKey(b.GetInstanceID()))
                        return 0; // Skip if either item transform is missing

                    var transformA = _itemTransforms[a.GetInstanceID()];
                    var transformB = _itemTransforms[b.GetInstanceID()];

                    if (transformA == null ||
                        transformB == null) return 0; // Skip if either item transform is destroyed

                    return Vector3.Distance(transform.position, transformA.position)
                        .CompareTo(Vector3.Distance(transform.position, transformB.position));
                }
            );

            var closestItem = _itemsInRange[0];
            if (CurrentPreviewedItem != closestItem)
            {
                CurrentPreviewedItem = closestItem;
                _previewManager.ShowPreview(CurrentPreviewedItem);
            }

            _isSorting = false;
        }


        public void RegisterItem(InventoryItem item)
        {
            if (!_itemsInRange.Contains(item)) _itemsInRange.Add(item);
            Debug.Log("Item registered");
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
        public void HideSelectedItemPreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
            _previewManager.HidePreview();
        }
    }
}
