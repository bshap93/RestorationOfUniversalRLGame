using Core.Events;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using Gameplay.ItemsInteractions;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Crafting.Cooking
{
    public class CookingStationController : MonoBehaviour, ISelectableTrigger, ICraftingStation,
        MMEventListener<RecipeEvent>, MMEventListener<RecipeGroupEvent>
    {
        public CraftingButtons craftingButtons;

        [Header("UI")] public Canvas stationCanvas;
        [FormerlySerializedAs("cookingUIPanel")]
        public CanvasGroup cookingChoiceUIPanel;
        public CanvasGroup cookingProgressUIDisplayler;
        public MMFeedbacks lightFireFeedback;

        [Header("Input")] public KeyCode interactionKey = KeyCode.F;
        [FormerlySerializedAs("stationRecipes")]
        [FormerlySerializedAs("_stationRecipes")]
        [Header("Station Setup")]
        [SerializeField]
        RecipeGroup stationSetRecipes;

        [FormerlySerializedAs("StationHasSetRecipes")]
        public bool stationHasSetRecipes;

        bool _isCookStaionLit;

        bool _isInPlayerRange;

        void Awake()
        {
            // Start with UI hidden
            HideStationChoicePanel();
            HideCookingProgressDisplay();

            // Setup crafting buttons
            if (craftingButtons != null) craftingButtons.SetCraftRecipes(stationSetRecipes);
        }

        void Start()
        {
            if (!stationHasSetRecipes) RecreateCraftingOptionsWithNewRecipes();
        }

        void Update()
        {
            if (_isInPlayerRange && Input.GetKeyDown(interactionKey)) ShowStationChoicePanel();
        }

        void OnEnable()
        {
            this.MMEventStartListening<RecipeEvent>();
            this.MMEventStartListening<RecipeGroupEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<RecipeEvent>();
            this.MMEventStopListening<RecipeGroupEvent>();
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
                HideStationChoicePanel();
            }
        }

        public void OnValidate()
        {
            if (stationSetRecipes == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No recipes assigned!", this);

            if (craftingButtons == null)
                Debug.LogWarning($"CookingStation '{gameObject.name}': No crafting buttons assigned!", this);
        }

        public void ShowStationChoicePanel()
        {
            if (stationCanvas != null) stationCanvas.enabled = true;
            if (cookingChoiceUIPanel != null)
            {
                cookingChoiceUIPanel.alpha = 1;
                cookingChoiceUIPanel.interactable = true;
                cookingChoiceUIPanel.blocksRaycasts = true;
            }

            if (craftingButtons != null) craftingButtons.gameObject.SetActive(true);
        }

        public void HideStationChoicePanel()
        {
            if (stationCanvas != null) stationCanvas.enabled = false;
            if (cookingChoiceUIPanel != null)
            {
                cookingChoiceUIPanel.alpha = 0;
                cookingChoiceUIPanel.interactable = false;
                cookingChoiceUIPanel.blocksRaycasts = false;
            }

            if (craftingButtons != null) craftingButtons.gameObject.SetActive(false);
        }
        public void OnSelectedItem()
        {
            ShowStationChoicePanel();
        }
        public void OnUnSelectedItem()
        {
            HideStationChoicePanel();
        }

        public void OnMMEvent(RecipeEvent eventType)
        {
            if (eventType.EventType == RecipeEventType.CraftingStarted)
                if (!_isCookStaionLit)
                {
                    lightFireFeedback?.PlayFeedbacks();
                    _isCookStaionLit = true;
                }

            if (eventType.EventType == RecipeEventType.CraftingFinished) HideStationChoicePanel();
        }
        public void OnMMEvent(RecipeGroupEvent eventType)
        {
            if (eventType.EventType == RecipeGroupEventType.RecipeGroupLearned)
            {
                RecreateCraftingOptionsWithNewRecipes();
                Debug.Log("Recreated crafting options with new recipes");
            }
        }
        void RecreateCraftingOptionsWithNewRecipes()
        {
            var craftableRecipesGroup = CraftingRecipeManager.ConvertToRecipeGroup(
                CraftingRecipeManager.GetAllKnownTTypeRecipes(RecipeType.Cooking).ToArray(), "AllKnownRecipes");

            stationSetRecipes = craftableRecipesGroup;
            craftingButtons.SetCraftRecipes(stationSetRecipes);

            craftingButtons.CreateButtons();
        }
        void HideCookingProgressDisplay()
        {
            Debug.Log("Hide Cooking Progress Display");
        }

        void ShowCookingProgressDisplay()
        {
            Debug.Log("Show Cooking Progress Display");
        }
    }
}
