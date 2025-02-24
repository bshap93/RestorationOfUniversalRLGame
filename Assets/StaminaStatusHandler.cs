using Core.Events;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

public class StaminaStatusHandler : CharacterAbility, MMEventListener<PlayerStatusEvent>
{
    public MMFeedbacks OutOfStaminaFeedback;
    public MMFeedbacks RegainedStaminaFeedback;
    [FormerlySerializedAs("OutOfStamina")] public bool outOfStamina;

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
        if (eventType.EventType == PlayerStatusEventType.OutOfStamina)
        {
            outOfStamina = true;

            _condition.ChangeState(CharacterStates.CharacterConditions.OutOfStamina);
            Debug.Log("Out of Stamina");
        }

        if (eventType.EventType == PlayerStatusEventType.RegainedStamina)

        {
            outOfStamina = false;
            _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            Debug.Log("Gained Stamina");
        }
    }
}
