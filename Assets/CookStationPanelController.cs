using System;
using System.Collections.Generic;
using HighlightPlus;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;
using UnityEngine.Serialization;

public class CookStationPanelController : MonoBehaviour, MMEventListener<MMInventoryEvent>,
    MMEventListener<CookingStationEvent>, MMEventListener<MMGameEvent>
{
    [FormerlySerializedAs("CookingStationPanel")]
    public GameObject cookingStationPanel;
    public MMProgressBar fuelBurntProgressBar;
    public MMProgressBar cookingProgressBar;
    readonly Stack<CookingStationController> _cookingStationControllers = new();

    [Obsolete("Obsolete")]
    void Start()
    {
        var cookingStationControllers = FindObjectsOfType<CookingStationController>();

        if (cookingStationControllers.Length == 0) cookingStationPanel.SetActive(false);

        HidePanel();
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
            if (cookingStationEvent.CookingStationControllerParameter == null)
            {
                Debug.LogError("CookingStationController is null");
                return;
            }


            _cookingStationControllers.Push(cookingStationEvent.CookingStationControllerParameter);
        }

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationOutOfRange)
        {
            if (_cookingStationControllers.Count == 0) throw new Exception("CookingStationController stack is empty");


            _cookingStationControllers.Pop();

            if (_cookingStationControllers.Count == 0)
                HidePanel();
        }

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationSelected)
            if (cookingStationEvent.CookingStationControllerParameter.IsPlayerInRange())
            {
                Debug.Log("About to call ShowPanel");
                ShowPanel();
            }


        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationDeselected)
            HidePanel();
    }
    public void OnMMEvent(MMGameEvent cookingStationEvent)
    {
        if (cookingStationEvent.EventName == "UpdateFuelProgressBar")
        {
            if (_cookingStationControllers.Count == 0) return;
            var cookingStationController = _cookingStationControllers.Peek();
            if (cookingStationEvent.StringParameter == cookingStationController.CookingStation.CraftingStationId)
                fuelBurntProgressBar.BarProgress = cookingStationEvent.Vector2Parameter.x;
            else
                Debug.LogError(
                    "Invalid fuel update: " + cookingStationEvent.StringParameter + " " +
                    cookingStationController.CookingStation.CraftingStationId);
        }
        
        if (cookingStationEvent.EventName == "UpdateCookingProgressBar") {
            if (_cookingStationControllers.Count == 0) return;
            var cookingStationController = _cookingStationControllers.Peek();
            if (cookingStationEvent.StringParameter == cookingStationController.CookingStation.CraftingStationId)
                cookingProgressBar.BarProgress = cookingStationEvent.Vector2Parameter.x;
            else
                Debug.LogError(
                    "Invalid cooking update: " + cookingStationEvent.StringParameter + " " +
                    cookingStationController.CookingStation.CraftingStationId);
        }
    }


    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        if (_cookingStationControllers.Count == 0) return;

        var topCookingStationController = _cookingStationControllers.Peek();

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens
            && topCookingStationController.IsPlayerInRange())
            ShowPanel();

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
        {
            HidePanel();
            Debug.Log("Now calling UnselectObject on: " + _cookingStationControllers.Peek().gameObject.name);
            HighlightManager.instance.UnselectObject(_cookingStationControllers.Peek().gameObject.transform);
        }
    }

    public void StartCooking()
    {
        var topCookingStationController = _cookingStationControllers.Peek();
        var queueInventory = topCookingStationController.GetQueueInventory();
        queueInventory.StartCookingCurrentRecipe();
    }

    void ShowPanel()
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

    void HidePanel()
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
