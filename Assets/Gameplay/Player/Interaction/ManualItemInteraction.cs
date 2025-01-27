using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using Project.UI.HUD;
using UnityEngine;

namespace Gameplay.Player.Interaction
{
    public class ManualItemInteraction : MonoBehaviour
    {
        public BaseItem Item; // The item to interact with
        bool _isInRange;
        PromptManager _promptManager;

        void Start()
        {
            _promptManager = FindFirstObjectByType<PromptManager>();
            if (_promptManager == null)
                Debug.LogWarning("PromptManager not found in the scene.");
        }

        void Update()
        {
            if (_isInRange && Input.GetKeyDown(KeyCode.F))
            {
                if (Item == null)
                {
                    Debug.LogWarning("No item assigned for interaction.");
                    return;
                }

                // Explicitly check the UsageType to prevent picking up when Usable
                if (Item.UsageType == ItemUsageType.Usable)
                    UseItem();
                else if (Item.UsageType == ItemUsageType.Pickable) PickItem();
            }
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInRange = true;
                if (Item != null)
                {
                    if (Item.UsageType == ItemUsageType.Pickable)
                        _promptManager?.ShowPickupPrompt();
                    else if (Item.UsageType == ItemUsageType.Usable) _promptManager?.ShowUsePrompt();
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInRange = false;
                _promptManager?.HidePickupPrompt();
                _promptManager?.HideUsePrompt();
            }
        }

        void PickItem()
        {
            Debug.Log($"Picked up: {Item.ItemName}");

            // Add the item to the player's inventory (if applicable)
            // Example: _inventory.Add(Item);

            Destroy(gameObject); // Remove the item from the scene
        }

        void UseItem()
        {
            Debug.Log($"Using: {Item.ItemName}");

            // Call the item's custom Use logic
            Item.Use("Player1");

            // Remove the item only if it should disappear
            if (Item.DisappearAfterUse) Destroy(gameObject);
        }
    }
}
