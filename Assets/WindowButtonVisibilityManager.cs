using Michsky.MUIP;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class WindowButtonVisibilityManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    public WindowManager windowManager; // Reference to the WindowManager

    void OnEnable()
    {
        this.MMEventStartListening();

        SetButtonVisibility(false);
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMInventoryEvent @event)
    {
        if (windowManager == null)
        {
            Debug.LogWarning("WindowManager is not assigned!");
            return;
        }

        switch (@event.InventoryEventType)
        {
            case MMInventoryEventType.InventoryOpens:
                SetButtonVisibility(true);
                break;

            case MMInventoryEventType.InventoryCloses:
                SetButtonVisibility(false);
                break;
        }
    }

    void SetButtonVisibility(bool visible)
    {
        foreach (var window in windowManager.windows)
            if (window.buttonObject != null)
                window.buttonObject.SetActive(visible);
    }
}
