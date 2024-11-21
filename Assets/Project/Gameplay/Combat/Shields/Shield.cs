using System;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Shields
{
    public class Shield : MonoBehaviour
    {
        public enum ShieldStates
        {
            Inactive,
            Starting,
            Active,
            Blocking,
            Broken,
            Interrupted
        }

        public ShieldProtectionArea ShieldProtectionArea;


        [Header("Shield Properties")] [Tooltip("Maximum health of the shield before it breaks")]
        public float MaxShieldHealth = 100f;
        [Tooltip("Time shield takes to recover after being broken")]
        public float RecoveryTime = 2f;
        [Tooltip("Amount of stamina consumed per block")]
        public float StaminaConsumption = 10f;

        [Header("Feedbacks")] public MMFeedbacks ShieldRaiseFeedback;
        public MMFeedbacks ShieldLowerFeedback;
        public MMFeedbacks ShieldBlockFeedback;
        public MMFeedbacks ShieldBreakFeedback;
        protected Animator _animator;
        protected CharacterHandleShield _handler;

        protected Character _owner;
        protected float _recoveryTimer;
        protected int _shieldStateParameter;

        public float CurrentShieldHealth { get; protected set; }
        public ShieldStates CurrentState { get; protected set; }

        protected virtual void Update()
        {
            // Handle state transitions
            switch (CurrentState)
            {
                case ShieldStates.Starting:
                    CurrentState = ShieldStates.Active;
                    UpdateAnimator();
                    break;

                case ShieldStates.Blocking:
                    CurrentState = ShieldStates.Active;
                    UpdateAnimator();
                    break;

                case ShieldStates.Broken:
                    if (_recoveryTimer > 0)
                    {
                        _recoveryTimer -= Time.deltaTime;
                    }
                    else
                    {
                        CurrentState = ShieldStates.Inactive;
                        CurrentShieldHealth = MaxShieldHealth;
                        UpdateAnimator();
                    }

                    break;
            }
        }

        public event Action<bool> OnShieldRaised;

        public virtual void SetOwner(Character owner, CharacterHandleShield handler)
        {
            _owner = owner;
            _handler = handler;
            _animator = _handler.CharacterAnimator;
        }

        public virtual void Initialization()
        {
            CurrentShieldHealth = MaxShieldHealth;
            CurrentState = ShieldStates.Inactive;
            _shieldStateParameter = Animator.StringToHash("ShieldState");
            InitializeFeedbacks();
        }

        protected virtual void InitializeFeedbacks()
        {
            ShieldRaiseFeedback?.Initialization(gameObject);
            ShieldLowerFeedback?.Initialization(gameObject);
            ShieldBlockFeedback?.Initialization(gameObject);
            ShieldBreakFeedback?.Initialization(gameObject);
        }

        public virtual void RaiseShield()
        {
            if (CurrentState != ShieldStates.Inactive) return;



            CurrentState = ShieldStates.Starting;
            UpdateAnimator();
            ShieldRaiseFeedback?.PlayFeedbacks();
            OnShieldRaised?.Invoke(true); // Raise event for animation feedback

            if (ShieldProtectionArea != null) ShieldProtectionArea.ShieldIsActive = true;
        }

        public virtual void LowerShield()
        {
            if (CurrentState == ShieldStates.Inactive || CurrentState == ShieldStates.Broken) return;


            CurrentState = ShieldStates.Inactive;
            UpdateAnimator();
            ShieldLowerFeedback?.PlayFeedbacks();
            OnShieldRaised?.Invoke(false); // Raise event for animation feedback

            if (ShieldProtectionArea != null) ShieldProtectionArea.ShieldIsActive = false;
        }

        public virtual void ProcessDamage(float damage)
        {
            if (CurrentState != ShieldStates.Active && CurrentState != ShieldStates.Starting) return;

            CurrentState = ShieldStates.Blocking;
            UpdateAnimator();
            ShieldBlockFeedback?.PlayFeedbacks();

            CurrentShieldHealth -= damage;
            if (CurrentShieldHealth <= 0) BreakShield();
        }

        protected virtual void BreakShield()
        {
            CurrentState = ShieldStates.Broken;
            UpdateAnimator();
            ShieldBreakFeedback?.PlayFeedbacks();
            _recoveryTimer = RecoveryTime;
        }

        protected virtual void UpdateAnimator()
        {
            if (_animator != null) _animator.SetInteger(_shieldStateParameter, (int)CurrentState);
        }
    }
}
