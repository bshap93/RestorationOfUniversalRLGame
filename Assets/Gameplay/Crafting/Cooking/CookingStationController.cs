using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using UnityEngine;

namespace Gameplay.Crafting.Cooking
{
    public class CookingStationController : MonoBehaviour
    {
        [Header("Station Setup")] public Craft stationRecipes;
        public CraftingButtons craftingButtons;

        [Header("UI")] public Canvas stationCanvas;
        public CanvasGroup cookingUIPanel;

        [Header("Input")] public KeyCode interactionKey = KeyCode.F;

        bool _isInPlayerRange;

        void Awake()
        {
            // Start with UI hidden
            HideStation();

            // Setup crafting buttons
            if (craftingButtons != null) craftingButtons.craftRecipes = stationRecipes;
        }

        void Update()
        {
            if (_isInPlayerRange && Input.GetKeyDown(interactionKey)) ShowStation();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = true;
                Debug.Log($"Station '{gameObject.name}': Press {interactionKey} to interact");
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isInPlayerRange = false;
                HideStation();
            }
        }

        void OnValidate()
        {
            if (stationRecipes == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No recipes assigned!", this);

            if (craftingButtons == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No crafting buttons assigned!", this);
        }

        public void ShowStation()
        {
            if (stationCanvas != null) stationCanvas.enabled = true;
            if (cookingUIPanel != null)
            {
                cookingUIPanel.alpha = 1;
                cookingUIPanel.interactable = true;
                cookingUIPanel.blocksRaycasts = true;
            }

            if (craftingButtons != null) craftingButtons.gameObject.SetActive(true);
        }

        public void HideStation()
        {
            if (stationCanvas != null) stationCanvas.enabled = false;
            if (cookingUIPanel != null)
            {
                cookingUIPanel.alpha = 0;
                cookingUIPanel.interactable = false;
                cookingUIPanel.blocksRaycasts = false;
            }

            if (craftingButtons != null) craftingButtons.gameObject.SetActive(false);
        }
    }
}
