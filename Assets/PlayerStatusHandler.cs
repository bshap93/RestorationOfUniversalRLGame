using Core.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using UnityEngine;

public class PlayerStatusHandler : MonoBehaviour, MMEventListener<PlayerStatusEvent>
{
    [SerializeField] MMFeedbacks OutOfStaminaFeedback;
    public bool OutOfStamina;

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }


    public void OnMMEvent(PlayerStatusEvent eventType)
    {
        if (eventType.EventType == PlayerStatusEventType.OutOfStamina) OutOfStamina = true;
    }
}
