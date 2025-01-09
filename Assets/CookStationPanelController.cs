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
        HidePanel();
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
                ShowPanel();


        if (cookingStationEvent.EventType == CookingStationEventType.CookingStationDeselected)
            HidePanel();
    }


    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        if (_cookingStationControllers.Count == 0) return;

        var topCookingStationController = _cookingStationControllers.Peek();

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens
            && topCookingStationController.IsPlayerInRange())
            ShowPanel();

        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            HidePanel();
    }

    void ShowPanel()
    {
        var canvasGroup = CookingStationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    void HidePanel()
    {
        var canvasGroup = CookingStationPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
