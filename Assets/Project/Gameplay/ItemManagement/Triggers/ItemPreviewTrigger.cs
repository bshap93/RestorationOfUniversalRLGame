using MoreMountains.InventoryEngine;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    public class ItemPreviewTrigger : MonoBehaviour
    {
        public InventoryItem Item; // Assign the InventoryItem to display

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var previewManager = other.GetComponent<PlayerItemPreviewManager>();
                previewManager.RegisterItem(Item);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var previewManager = other.GetComponent<PlayerItemPreviewManager>();
                previewManager.UnregisterItem(Item);
            }
        }
    }
}
