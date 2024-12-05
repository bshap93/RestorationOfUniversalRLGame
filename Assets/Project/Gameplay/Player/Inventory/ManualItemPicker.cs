using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using Project.Gameplay.ItemManagement;
using Project.UI.HUD;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Player.Inventory
{
    public class ManualItemPicker : MonoBehaviour
    {
        public InventoryItem Item; // The item to be picked up
        public int Quantity = 1;

        [FormerlySerializedAs("PickedMMFeedbacks")]
        [Header("Feedbacks")]
        [Tooltip("Feedbacks to play when the item is picked up")]
        public MMFeedbacks pickedMmFeedbacks; // Feedbacks to play when the item is picked up

        bool _isInRange;
        PreviewManager _previewManager;
        PromptManager _promptManager;
        MoreMountains.InventoryEngine.Inventory _targetInventory;

        void Start()
        {
            _promptManager = FindObjectOfType<PromptManager>();
            if (_promptManager == null) Debug.LogWarning("PickupPromptManager not found in the scene.");

            _previewManager = FindObjectOfType<PreviewManager>();
            if (_previewManager == null) Debug.LogWarning("PlayerItemPreviewManager not found in the scene.");


            // Locate PortableSystems and retrieve the appropriate inventory
            var portableSystems = GameObject.Find("PortableSystems");
            if (portableSystems != null)
                _targetInventory = portableSystems.GetComponentInChildren<MoreMountains.InventoryEngine.Inventory>();

            if (_targetInventory == null) Debug.LogWarning("Target inventory not found in PortableSystems.");

            // Initialize feedbacks
            if (pickedMmFeedbacks != null) pickedMmFeedbacks.Initialization(gameObject);
        }

        void Update()
        {
            if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) PickItem();
        }

        void OnTriggerEnter(Collider itemPickerCollider)
        {
            if (itemPickerCollider.CompareTag("Player"))
            {
                _isInRange = true;
                _promptManager?.ShowPickupPrompt();
                _promptManager?.ShowPreviewPanel(Item); // Show preview when entering
            }
        }

        void OnTriggerExit(Collider collider)
        {
            if (collider.CompareTag("Player"))
            {
                _isInRange = false;
                _promptManager?.HidePickupPrompt();
                _promptManager?.HidePreviewPanel(); // Ensure preview hides on exit
            }
        }

        void PickItem()
        {
            if (Item == null || _targetInventory == null || _previewManager.CurrentPreviewedItem != Item) return;

            if (_targetInventory.AddItem(Item, Quantity))
            {
                // Hide the prompt and preview panel on successful pickup
                _promptManager?.HidePickupPrompt();
                // _pickupPromptManager?.HidePreviewPanel();

                // Play feedbacks on successful pickup
                if (pickedMmFeedbacks != null) pickedMmFeedbacks.PlayFeedbacks();


                // Inform PlayerItemPreviewManager to update the preview
                var playerItemPreviewManager = FindObjectOfType<PlayerItemPreviewManager>();
                if (playerItemPreviewManager != null)
                    playerItemPreviewManager.UnregisterItem(GetComponent<ItemPreviewTrigger>());


                Destroy(gameObject);
            }
            else
            {
                ShowInventoryFullMessage();
            }
        }

        void ShowInventoryFullMessage()
        {
            Debug.Log("Inventory is full or item cannot be picked up.");
            // Additional UI feedback for full inventory, if needed
        }
    }
}
