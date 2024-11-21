using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using Project.Gameplay.ItemManagement;
using Project.UI.HUD;
using UnityEngine;

namespace Project.Gameplay.Player.Inventory
{
    public class PlayerItemPreviewManager : MonoBehaviour
    {
        readonly List<ItemPreviewTrigger> nearbyItems = new();
        PreviewManager _previewManager;
        ItemPreviewTrigger currentItem;
        public InventoryItem CurrentPreviewedItem => currentItem?.Item; // Exposes the current item's InventoryItem


        void Start()
        {
            _previewManager = FindObjectOfType<PreviewManager>();
            if (_previewManager == null) Debug.LogWarning("PreviewManager not found in the scene.");
        }

        void Update()
        {
            DisplayNearestItem();
        }

        void DisplayNearestItem()
        {
            // Remove null or destroyed items from the list
            nearbyItems.RemoveAll(item => item == null);

            if (nearbyItems.Count == 0 || _previewManager == null)
            {
                if (currentItem != null)
                {
                    _previewManager.HidePreview();
                    currentItem = null;
                }

                return;
            }

            // Sort to get the closest item
            nearbyItems.Sort(
                (a, b) =>
                    Vector3.Distance(transform.position, a.transform.position)
                        .CompareTo(Vector3.Distance(transform.position, b.transform.position)));

            // Only update if the closest item has changed
            var closestItem = nearbyItems[0];
            if (closestItem != currentItem)
            {
                _previewManager.ShowPreview(closestItem.Item);
                currentItem = closestItem;
            }
        }


        public void RegisterItem(ItemPreviewTrigger item)
        {
            if (!nearbyItems.Contains(item)) nearbyItems.Add(item);
        }

        public void UnregisterItem(ItemPreviewTrigger item)
        {
            if (nearbyItems.Contains(item))
            {
                nearbyItems.Remove(item);

                // Reset current item if it was removed
                if (currentItem == item)
                {
                    _previewManager.HidePreview();
                    currentItem = null;
                }
            }
        }
    }
}
