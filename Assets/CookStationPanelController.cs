using System;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;

public class CookStationPanelController : MonoBehaviour, MMEventListener<MMInventoryEvent>,
    MMEventListener<CookingStationEvent>
{
    public GameObject CookingStationPanel;
    readonly Stack<CookingStationController> _cookingStationControllers = new();
    void Start()
    {
        CookingStationPanel.SetActive(false);
    }

    void OnEnable()
    {
        this.MMEventStartListening<MMInventoryEvent>();

        this.MMEventStartListening<CookingStationEvent>();
    }


    void OnDisable()
    {
        this.MMEventStopListening<MMInventoryEvent>();

        this.MMEventStopListening<CookingStationEvent>();
    }
    public void OnMMEvent(CookingStationEvent cookingStationEvent)
    {
        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationInRange)
        {
            Debug.Log("Cooking station in range");
            if (cookingStationEvent.CookingStationControllerParameter == null)
            {
                Debug.LogError("CookingStationController is null");
                return;
            }

            Debug.Log("CSCP not null");

            _cookingStationControllers.Push(cookingStationEvent.CookingStationControllerParameter);

            Debug.Log("Cooking station added: " + cookingStationEvent.CookingStationControllerParameter.name);
        }

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationOutOfRange)
        {
            if (_cookingStationControllers.Count == 0) throw new Exception("CookingStationController stack is empty");

            Debug.Log("Cooking station out of range");

            _cookingStationControllers.Pop();

            if (_cookingStationControllers.Count == 0)
                CookingStationPanel.SetActive(false);
        }

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationSelected)
            if (cookingStationEvent.CookingStationControllerParameter.IsPlayerInRange())
                CookingStationPanel.SetActive(true);

        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationDeselected)
            CookingStationPanel.SetActive(false);
    }


    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        if (_cookingStationControllers.Count == 0) return;

        var topCookingStationController = _cookingStationControllers.Peek();

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens
            && topCookingStationController.IsPlayerInRange())
            CookingStationPanel.SetActive(true);

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            CookingStationPanel.SetActive(false);
    }
}
