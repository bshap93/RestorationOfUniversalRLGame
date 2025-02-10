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

    public void OnMMEvent(MMInventoryEvent mmEvent)
    {
        if (mmEvent.InventoryEventType == MMInventoryEventType.InventoryOpens)
            // Show the UI buttons
            gameObject.SetActive(true);
        else if (mmEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            // Hide the UI buttons
            gameObject.SetActive(false);
    }
}
