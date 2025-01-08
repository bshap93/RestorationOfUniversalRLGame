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

    public void OnMMEvent(MMInventoryEvent inventoryEvent)
    {
        if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens)
            // Show the UI buttons
            gameObject.SetActive(true);
        else if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            // Hide the UI buttons
            gameObject.SetActive(false);
    }
}
