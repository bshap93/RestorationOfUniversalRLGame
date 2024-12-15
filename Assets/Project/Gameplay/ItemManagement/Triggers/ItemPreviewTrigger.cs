using MoreMountains.InventoryEngine;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    public class ItemPreviewTrigger : MonoBehaviour
    {
        public InventoryItem Item; // Assign the InventoryItem to display

        PlayerItemPreviewManager _previewManager;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_previewManager == null)
                    _previewManager = other.GetComponent<PlayerItemPreviewManager>();

                _previewManager.RegisterItem(Item);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_previewManager == null)
                    _previewManager = other.GetComponent<PlayerItemPreviewManager>();

                _previewManager.UnregisterItem(Item);
            }
        }

        public void OnSelectedItem()
        {
            if (_previewManager == null)
            {
                _previewManager = FindObjectOfType<PlayerItemPreviewManager>();
                Debug.LogWarning("PreviewManager not found in the scene.");
            }

            _previewManager.RegisterItem(Item);
            _previewManager.ShowSelectedItemPreviewPanel(Item);
        }

        public void OnUnSelectedItem()
        {
            if (_previewManager == null)
                _previewManager = FindObjectOfType<PlayerItemPreviewManager>();

            _previewManager.UnregisterItem(Item);
            _previewManager.HideSelectedItemPreviewPanel();
        }
    }
}
