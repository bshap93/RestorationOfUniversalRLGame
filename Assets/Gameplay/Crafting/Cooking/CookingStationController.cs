using Gameplay.Events;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Crafting.Cooking
{
    public class CookingStationController : MonoBehaviour, ISelectableTrigger, ICraftingStation,
        MMEventListener<CraftingEvent>
    {
        [Header("Station Setup")] public Craft stationRecipes;
        public CraftingButtons craftingButtons;

        [Header("UI")] public Canvas stationCanvas;
        [FormerlySerializedAs("cookingUIPanel")]
        public CanvasGroup cookingChoiceUIPanel;
        public CanvasGroup cookingProgressUIDisplayler;
        public MMFeedbacks lightFireFeedback;

        [Header("Input")] public KeyCode interactionKey = KeyCode.F;

        bool _isCookStaionLit;

        bool _isInPlayerRange;


        void Awake()
        {
            // Start with UI hidden
            HideStationChoicePanel();
            HideCookingProgressDisplay();

            // Setup crafting buttons
            if (craftingButtons != null) craftingButtons.SetCraftRecipes(stationRecipes);
        }
        void HideCookingProgressDisplay()
        {
            throw new System.NotImplementedException();
        }
        
        void ShowCookingProgressDisplay()
        {
            throw new System.NotImplementedException();
        }

        void Update()
        {
            if (_isInPlayerRange && Input.GetKeyDown(interactionKey)) ShowStationChoicePanel();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
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
            if (stationRecipes == null)
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

        public void OnMMEvent(CraftingEvent eventType)
        {
            if (eventType.EventType == CraftingEventType.CraftingStarted)
                if (!_isCookStaionLit)
                {
                    lightFireFeedback?.PlayFeedbacks();
                    _isCookStaionLit = true;
                }

            if (eventType.EventType == CraftingEventType.CraftingFinished) HideStationChoicePanel();
        }
    }
}
