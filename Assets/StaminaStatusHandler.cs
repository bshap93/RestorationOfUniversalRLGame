using Core.Events;
using Gameplay.Player.Stats;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

public class StaminaStatusHandler : CharacterAbility, MMEventListener<PlayerStatusEvent>
{
    public enum CharacterStatusConditions
    {
        OutOfStamina,
        Normal
    }

    [FormerlySerializedAs("_altCharacterHandleWeapon")] [SerializeField]
    AltCharacterHandleWeapon altCharacterHandleWeapon;


    public MMFeedbacks OutOfStaminaFeedback;
    public MMFeedbacks RegainedStaminaFeedback;

    CharacterStatusConditions _currentStatusCondition;

    protected override void Start()
    {
        base.Start();
        if (PlayerStaminaManager.StaminaPoints <= 0)
            _currentStatusCondition = CharacterStatusConditions.OutOfStamina;
        else
            _currentStatusCondition = CharacterStatusConditions.Normal;

        if (altCharacterHandleWeapon != null)
            altCharacterHandleWeapon.SetPlayerStatusCondition(_currentStatusCondition);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        this.MMEventStartListening();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        this.MMEventStopListening();
    }


    public void OnMMEvent(PlayerStatusEvent eventType)
    {
        if (eventType.EventType == PlayerStatusEventType.OutOfStamina)
        {
            OutOfStaminaFeedback?.PlayFeedbacks();
            _currentStatusCondition = CharacterStatusConditions.OutOfStamina;
            Debug.Log("Out of Stamina");
        }

        if (eventType.EventType == PlayerStatusEventType.RegainedStamina)

        {
            // _condition.ChangeState(CharacterStates.CharacterConditions.Normal);
            RegainedStaminaFeedback?.PlayFeedbacks();
            _currentStatusCondition = CharacterStatusConditions.Normal;
            Debug.Log("Gained Stamina");
        }
    }
    public CharacterStatusConditions GetStatusCondition()
    {
        return _currentStatusCondition;
    }
}
