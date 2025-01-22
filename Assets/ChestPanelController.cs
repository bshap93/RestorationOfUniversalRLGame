using Core.Events;
using MoreMountains.Tools;
using UnityEngine;

public class ChestPanelController : MonoBehaviour, MMEventListener<ContainerEvent>
{
    public GameObject chestPanelPrefab;
    public GameObject chestPanel;
    ChestPanelInstance _chestPanelInstance;

    void Start()
    {
        Initialize();
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(ContainerEvent mmEvent)
    {
        if (mmEvent.EventType == ContainerEventType.ContainerInRange)
        {
            var controller = mmEvent.ContainerControllerParameter;
            if (controller == null)
            {
                Debug.LogError("Container controller is null");
                return;
            }

            // Instantiate as a child of this object and set position
            chestPanel = Instantiate(chestPanelPrefab, transform);
            _chestPanelInstance = chestPanel.GetComponent<ChestPanelInstance>();

            // Set Controller 
            _chestPanelInstance.containerController = controller;

            // Set Inventory
            if (controller.GetInventory() != null)
                _chestPanelInstance.SetInventory(controller.GetInventory());
        }
    }

    void Initialize()
    {
    }

    void ShowPanel()
    {
        var canvasGroup = chestPanel.GetComponent<CanvasGroup>();
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
        var canvasGroup = chestPanel.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            Debug.Log("HidePanel called, setting canvasGroup.alpha to 0");
            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
}
