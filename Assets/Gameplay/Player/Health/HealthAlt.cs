using System;
using System.Collections.Generic;
using Core.Events;
using Gameplay.Combat.Shields;
using Gameplay.Player.Stats;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Shields;
using UnityEngine;

namespace Gameplay.Player.Health
{
    [Serializable]
    public class HealthAlt : MoreMountains.TopDownEngine.Health, MMEventListener<PlayerStatusEvent>,
        MMEventListener<HealthEvent>
    {
        [MMInspectorGroup("Health Status Change", true, 3)]
        public MMFeedbacks RecoveryFeedback;
        public MMFeedbacks FullyRecoverFeedback;
        public MMFeedbacks IncreaseMaximumHealthFeedback;
        public MMFeedbacks DecreaseMaximumHealthFeedback;
        GameObject _shield;
        Shield _shieldComponent;
        ShieldProtectionArea _shieldProtection;

        protected override void Awake()
        {
            base.Awake();
            InitialHealth = PlayerHealthManager.HealthPoints;
            MaximumHealth = PlayerHealthManager.HealthPoints;

            ShieldProtectionArea.OnShieldEquipped += AssignShield;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            this.MMEventStartListening<PlayerStatusEvent>();
            this.MMEventStartListening<HealthEvent>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            this.MMEventStopListening<PlayerStatusEvent>();
            this.MMEventStopListening<HealthEvent>();
        }

        void OnDestroy()
        {
            ShieldProtectionArea.OnShieldEquipped -= AssignShield;
        }
        public void OnMMEvent(HealthEvent eventType)
        {
            switch (eventType.EventType)
            {
                case HealthEventType.ConsumeHealth:
                    Damage(eventType.ByValue, null, 0, 0, Vector3.zero);
                    break;
                case HealthEventType.RecoverHealth:
                    SetHealth(CurrentHealth + eventType.ByValue);
                    RecoveryFeedback?.PlayFeedbacks();

                    break;
                case HealthEventType.FullyRecoverHealth:
                    CurrentHealth = MaximumHealth;
                    SetHealth(CurrentHealth);
                    FullyRecoverFeedback?.PlayFeedbacks();
                    break;
                case HealthEventType.IncreaseMaximumHealth:
                    IncreaseMaximumHealthFeedback?.PlayFeedbacks();
                    SetMaximumHealth(MaximumHealth + eventType.ByValue);
                    break;
                case HealthEventType.DecreaseMaximumHealth:
                    DecreaseMaximumHealthFeedback?.PlayFeedbacks();
                    SetMaximumHealth(MaximumHealth - eventType.ByValue);
                    break;
                case HealthEventType.Initialize:
                    break;
            }
        }
        public void OnMMEvent(PlayerStatusEvent eventType)
        {
            switch (eventType.EventType)
            {
                case PlayerStatusEventType.OutOfHealth:
                    Debug.Log("Should be dead");
                    break;
                case PlayerStatusEventType.RegainedHealth:
                    Debug.Log("Regained Health");
                    break;
                case PlayerStatusEventType.ImmuneToDamage:
                    ImmuneToDamage = true;

                    Debug.Log("Immune to damage");
                    break;
            }
        }


        public override void Damage(float damage, GameObject instigator, float flickerDuration,
            float invincibilityDuration, Vector3 damageDirection, List<TypedDamage> typedDamages = null)
        {
            // Check if the shield blocks the damage
            if (_shieldProtection != null && _shieldProtection.IsBlocking(instigator.transform.position))
            {
                Debug.Log(
                    $"Shield blocked damage from {instigator.name}, _shieldProtection: {_shieldProtection != null}, _shieldProtection.ISBlocking: {_shieldProtection.IsBlocking(instigator.transform.position)}");


                return; // Exit early if shield blocks damage
            }

            // Otherwise, apply damage as usual
            base.Damage(damage, instigator, flickerDuration, invincibilityDuration, damageDirection, typedDamages);
        }

        void AssignShield(ShieldProtectionArea shield)
        {
            _shieldProtection = shield;
            _shield = shield.gameObject;
            _shieldComponent = _shield.GetComponent<Shield>();
            Debug.Log("Shield assigned to ShieldedHealth.");
        }
        public void SetMaximumHealth(float savedMaxHealth)
        {
            MaximumHealth = savedMaxHealth;
        }
    }
}
