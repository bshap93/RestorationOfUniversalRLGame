using Gameplay.Events;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

public class NotificationListener : MonoBehaviour, MMEventListener<CraftingEvent>
{
    [FormerlySerializedAs("craftedNewItemNotification")] [SerializeField]
    CanvasGroup craftedNewItemNotificationCanvasGroup;
    [FormerlySerializedAs("_craftedNewItemNotification")] [SerializeField]
    CraftedNewItemNotification craftedNewItemNotification;

    void Start()
    {
        if (craftedNewItemNotificationCanvasGroup == null)
            craftedNewItemNotificationCanvasGroup = craftedNewItemNotification.gameObject.GetComponent<CanvasGroup>();

        Debug.Log("NotificationListener disabled craftedNewItemNotification");
        DisableCanvasGroup(craftedNewItemNotificationCanvasGroup, craftedNewItemNotification);
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }


    public void OnMMEvent(CraftingEvent craftingEvent)
    {
        if (craftingEvent.EventType == CraftingEventType.CraftingFinished)
            EnableCanvasGroup(craftedNewItemNotificationCanvasGroup, craftingEvent);
    }
    void DisableCanvasGroup(CanvasGroup canvasGroup, CraftedNewItemNotification craftedNewItemNotification1)
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        craftedNewItemNotification1.Hide();
    }

    void EnableCanvasGroup(CanvasGroup canvasGroup, CraftingEvent craftingEvent)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        craftedNewItemNotification.RestartWithNewItem(craftingEvent.Recipe.Item);
    }
}
