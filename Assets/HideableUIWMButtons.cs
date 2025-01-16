using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class HideableUIWMButtons : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    void Start()
    {
        // Hide the UI buttons
        gameObject.SetActive(false);
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMInventoryEvent @event)
    {
        if (@event.InventoryEventType == MMInventoryEventType.InventoryOpens)
            // Show the UI buttons
            gameObject.SetActive(true);
        else if (@event.InventoryEventType == MMInventoryEventType.InventoryCloses)
            // Hide the UI buttons
            gameObject.SetActive(false);
    }
}
