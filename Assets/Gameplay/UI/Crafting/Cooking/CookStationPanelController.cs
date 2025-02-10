using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.UI.Crafting.Cooking
{
    public class CookStationPanelController : MonoBehaviour, MMEventListener<MMInventoryEvent>,
        MMEventListener<CookingStationEvent>, MMEventListener<MMGameEvent>, MMEventListener<TopDownEngineEvent>
    {
        [SerializeField] List<GameObject> CookingStationPanels;
        [SerializeField] List<CookingStation> CookingStations;

        CookStationPanelInstance _cookingStationPanelInstance;
        GameObject _currentCookingStationPanel;
        [FormerlySerializedAs("_cookingStationPanels")] [SerializeField]
        Dictionary<CookingStation, GameObject> CookingStationPanelsDict;

        void Start()
        {
            if (CookingStationPanels.Count != CookingStations.Count)
            {
                Debug.LogError("Cooking station panels and cooking stations count mismatch");
                return;
            }

            CookingStationPanelsDict = new Dictionary<CookingStation, GameObject>();
            foreach (var cookingStation in CookingStations)
                CookingStationPanelsDict.Add(
                    cookingStation, CookingStationPanels[CookingStations.IndexOf(cookingStation)]);
        }

        void OnEnable()
        {
            this.MMEventStartListening<MMInventoryEvent>();

            this.MMEventStartListening<CookingStationEvent>();

            this.MMEventStartListening<MMGameEvent>();

            this.MMEventStartListening<TopDownEngineEvent>();
        }


        void OnDisable()
        {
            this.MMEventStopListening<MMInventoryEvent>();

            this.MMEventStopListening<CookingStationEvent>();

            this.MMEventStopListening<MMGameEvent>();

            this.MMEventStopListening<TopDownEngineEvent>();
        }
        public void OnMMEvent(CookingStationEvent mmEvent)
        {
            if (mmEvent.EventType == CookingStationEventType.CookingStationInRange)
            {
                var controller = mmEvent.CookingStationControllerParameter;
                if (controller == null)
                {
                    Debug.LogError("Cooking station controller is null");
                    return;
                }

                // Instantiate as a child of this object and set position
                // cookingStationPanel = Instantiate(cookingStationPanelPrefab, transform);
                _currentCookingStationPanel = CookingStationPanelsDict[controller.cookingStation];
                _cookingStationPanelInstance = _currentCookingStationPanel.GetComponent<CookStationPanelInstance>();

                // Set the controller first since other methods depend on it
                _cookingStationPanelInstance.cookingStationController = controller;

                // Then set the inventories
                if (controller.GetDepositInventory() != null)
                    _cookingStationPanelInstance.SetCookingDepositInventory(controller.GetDepositInventory());

                if (controller.GetQueueInventory() != null)
                    _cookingStationPanelInstance.SetCookingQueueInventory(controller.GetQueueInventory());

                if (controller.GetFuelInventory() != null)
                    _cookingStationPanelInstance.SetFuelInventory(controller.GetFuelInventory());

                // ShowPanel();
                HidePanel();
                return;
            }

            if (mmEvent.EventType == CookingStationEventType.CookingStationOutOfRange)
                // HidePanel();
                if (_currentCookingStationPanel != null)
                {
                    HidePanel();
                    _currentCookingStationPanel = null;
                    _cookingStationPanelInstance = null;
                }

            if (mmEvent.EventType == CookingStationEventType.CookingStationSelected) ShowPanel();

            if (mmEvent.EventType == CookingStationEventType.CookingStationDeselected) HidePanel();
        }
        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "UpdateFuelProgressBar")
            {
            }

            if (mmEvent.EventName == "UpdateCookingProgressBar")
            {
            }
        }


        public void OnMMEvent(MMInventoryEvent mmEvent)
        {
        }
        public void OnMMEvent(TopDownEngineEvent mmEvent)
        {
            if (mmEvent.EventType == TopDownEngineEventTypes.SpawnComplete)
            {
            }
        }


        void ShowPanel()
        {
            var canvasGroup = _currentCookingStationPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                Debug.Log("ShowPanel called, setting canvasGroup.alpha to 1");
                canvasGroup.alpha = 1;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            }
        }

        void HidePanel()
        {
            var canvasGroup = _currentCookingStationPanel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
            }
        }
    }
}
