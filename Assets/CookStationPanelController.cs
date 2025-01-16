using System.Collections.Generic;
using HighlightPlus;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Core.Events;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.Serialization;

public class CookStationPanelController : MonoBehaviour, MMEventListener<MMInventoryEvent>,
    MMEventListener<CookingStationEvent>, MMEventListener<MMGameEvent>
{
    public GameObject cookingStationPanelPrefab;
    [FormerlySerializedAs("_cookingStationPanels")] [SerializeField]
    List<GameObject> cookingStationPanels = new();

    GameObject _currentCookingStationPanel;

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();

        this.MMEventStartListening<CookingStationEvent>();

        this.MMEventStartListening<MMGameEvent>();
    }


    void OnDisable()
    {
        this.MMEventStopListening<MMInventoryEvent>();

        this.MMEventStopListening<CookingStationEvent>();

        this.MMEventStopListening<MMGameEvent>();
    }
    public void OnMMEvent(CookingStationEvent cookingStationEvent)
    {
        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationInRange)
        {
            ShowRelevantCookingStationPanel(cookingStationEvent);
            return;
        }

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationOutOfRange)
        {
            HideDeselectedCookingPanel(cookingStationEvent);
            return;
        }

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationSelected)
        {
            ShowRelevantCookingStationPanel(cookingStationEvent);
            return;
        }


        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationDeselected)
            HideDeselectedCookingPanel(cookingStationEvent);
    }
    public void OnMMEvent(MMGameEvent cookingStationEvent)
    {
        if (cookingStationEvent.EventName == "UpdateFuelProgressBar")
        {
            Debug.Log("StringParameter: " + cookingStationEvent.StringParameter);
            var relevantCookingStationPanel = cookingStationPanels.Find(
                cookingStationPanel =>
                    cookingStationPanel.GetComponent<CookStationPanelInstance>().cookingStationController.CookingStation
                        .CraftingStationId ==
                    cookingStationEvent.StringParameter);

            if (relevantCookingStationPanel == null)
            {
                Debug.LogError("RelevantCookingStationPanel is null");
                return;
            }

            var fuelProgressBar = relevantCookingStationPanel.GetComponent<CookStationPanelInstance>()
                .fuelBurntProgressBar;

            if (fuelProgressBar == null)
            {
                Debug.LogError("FuelProgressBar is null");
                return;
            }

            fuelProgressBar.BarProgress = cookingStationEvent.Vector2Parameter.x;
        }

        if (cookingStationEvent.EventName == "UpdateCookingProgressBar")
        {
            var relevantCookingStationPanel = cookingStationPanels.Find(
                cookingStationPanel =>
                    cookingStationPanel.GetComponent<CookStationPanelInstance>().cookingStationController.CookingStation
                        .CraftingStationId ==
                    cookingStationEvent.StringParameter);

            if (relevantCookingStationPanel == null)
            {
                Debug.LogError("RelevantCookingStationPanel is null");
                return;
            }

            var cookingProgressBar =
                relevantCookingStationPanel.GetComponent<CookStationPanelInstance>().cookingProgressBar;

            if (cookingProgressBar == null)
            {
                Debug.LogError("CookingProgressBar is null");
                return;
            }

            cookingProgressBar.BarProgress = cookingStationEvent.Vector2Parameter.x;
        }
    }


    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        // if (_cookingStationControllers.Count == 0) return;
        if (_currentCookingStationPanel == null) return;


        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens
           )
            if (_currentCookingStationPanel != null)
                ShowPanel(_currentCookingStationPanel);

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            if (_currentCookingStationPanel != null)
            {
                HidePanel(_currentCookingStationPanel);
                HighlightManager.instance.UnselectObject(_currentCookingStationPanel.transform);
            }
    }

    void Initialize()
    {
        var cookingStationControllers = FindObjectsByType<CookingStationController>(FindObjectsSortMode.None);
        if (cookingStationControllers.IsNullOrEmpty())
        {
            Debug.Log("CookingStationControllers is null or empty");
            return;
        }

        foreach (var cookingStationController in cookingStationControllers)
        {
            if (cookingStationPanelPrefab == null)
            {
                Debug.LogError("CookingStationController is null");
                return;
            }

            var cookingStationPanel = Instantiate(cookingStationPanelPrefab, transform);
            var cookingStationPanelInstance = cookingStationPanel.GetComponent<CookStationPanelInstance>();
            cookingStationPanelInstance.cookingStationController = cookingStationController;
            // cookingStationPanelInstance.cookingDepositInventory = cookingStationController.depositInventory;
            // cookingStationPanelInstance.cookingQueueInventory = cookingStationController.queueInventory;
            // cookingStationPanelInstance.fuelInventory = cookingStationController.fuelInventory;

            cookingStationPanelInstance.SetCookingDepositInventory(cookingStationController.GetDepositInventory());
            cookingStationPanelInstance.SetCookingQueueInventory(cookingStationController.GetQueueInventory());
            cookingStationPanelInstance.SetFuelInventory(cookingStationController.GetFuelInventory());


            cookingStationPanels.Add(cookingStationPanel);
            HidePanel(cookingStationPanel);
        }
    }
    void HideDeselectedCookingPanel(CookingStationEvent cookingStationEvent)
    {
        var currentStationId = cookingStationEvent.CookingStationControllerParameter.CookingStation.CraftingStationId;
        var eventStationId = cookingStationEvent.CookingStationControllerParameter.CookingStation.CraftingStationId;

        if (string.IsNullOrEmpty(currentStationId) || string.IsNullOrEmpty(eventStationId))
        {
            Debug.LogError("CurrentStationId or EventStationId is null or empty");
            return;
        }

        if (currentStationId != eventStationId)
        {
            Debug.LogError("CurrentStationId is not equal to EventStationId");
            return;
        }

        HidePanel(_currentCookingStationPanel);
    }
    void ShowRelevantCookingStationPanel(CookingStationEvent cookingStationEvent)
    {
        if (cookingStationEvent.CookingStationControllerParameter == null)
        {
            Debug.LogError("CookingStationController is null");
            return;
        }

        var cookingEventStationController = cookingStationEvent.CookingStationControllerParameter;
        var eventStationId = cookingEventStationController.CookingStation.CraftingStationId;

        if (string.IsNullOrEmpty(eventStationId))
        {
            Debug.LogError("EventStationId is null or empty");
            return;
        }


        _currentCookingStationPanel = cookingStationPanels.Find(
            cookingStationPanel =>
                cookingStationPanel.GetComponent<CookStationPanelInstance>().cookingStationController.CookingStation
                    .CraftingStationId ==
                eventStationId);


        // Hide all panels
        foreach (var cookingStationPanel in cookingStationPanels)
        {
            if (cookingStationPanel == null)
            {
                Debug.LogError("CookingStationPanel is null");
                return;
            }

            HidePanel(cookingStationPanel);
        }

        // Show the current panel
        ShowPanel(_currentCookingStationPanel);
    }


    void ShowPanel(GameObject cookingStationPanel)
    {
        var canvasGroup = cookingStationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log("ShowPanel called, setting canvasGroup.alpha to 1");
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void HidePanel(GameObject cookingStationPanel)
    {
        var canvasGroup = cookingStationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
