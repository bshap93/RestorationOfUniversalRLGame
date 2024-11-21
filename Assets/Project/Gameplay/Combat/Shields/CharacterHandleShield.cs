using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat.Shields
{
    public class CharacterHandleShield : CharacterAbility
    {
        [MMInspectorGroup("Shield", true, 10)] public Shield InitialShield;
        public Transform ShieldAttachment;
        public bool AutomaticallyBindAnimator = true;
        public int HandleShieldID = 1;

        [MMInspectorGroup("Input", true, 11)]
        /// if this is true, this ability can perform as usual, if not, it'll be ignored
        [Tooltip("if this is true, this ability can perform as usual, if not, it'll be ignored")]
        public bool InputAuthorized = true;
        /// if true, the shield will stay up as long as the button is held
        [Tooltip("if true, the shield will stay up as long as the button is held")]
        public bool ContinuousPress = true;
        public Animator CharacterAnimator;

        [FormerlySerializedAs("_shieldActive")]
        public bool shieldActive;

        AltCharacterHandleWeapon _altCharacterHandleWeapon;
        Shield _currentShield;


        public Shield CurrentShield { get; protected set; }

        protected override void Start()
        {
            base.Initialization();

            _altCharacterHandleWeapon = GetComponent<AltCharacterHandleWeapon>();
        }


        public override string HelpBoxText()
        {
            return "This ability allows the character to use shields for blocking damage. Uses the Interact button.";
        }

        protected override void Initialization()
        {
            base.Initialization();
            AssignRequiredComponents();
            SetupShield();
            if (_currentShield != null) _currentShield.OnShieldRaised += HandleShieldFeedback;
        }

        void HandleShieldFeedback(bool isRaised)
        {
            if (CharacterAnimator != null) CharacterAnimator.SetBool("ShieldUp", isRaised);
        }

        protected virtual void AssignRequiredComponents()
        {
            // Ensure we have an animator
            if (_animator == null && AutomaticallyBindAnimator)
            {
                _animator = GetComponent<Animator>();
                if (_animator == null) _animator = GetComponentInChildren<Animator>();
            }

            if (_animator == null) Debug.LogError("No animator found for CharacterHandleShield!", this);

            // Set up shield attachment point
            if (ShieldAttachment == null)
            {
                ShieldAttachment = transform;
                Debug.Log("Using transform as shield attachment point");
            }
        }

        protected virtual void SetupShield()
        {
            if (InitialShield != null) EquipShield(InitialShield);
        }

        public virtual void EquipShield(Shield newShield)
        {
            // Cleanup existing shield
            if (CurrentShield != null)
            {
                Destroy(CurrentShield.gameObject);
                CurrentShield = null;
            }

            if (newShield != null)
            {
                // Instantiate new shield
                var shieldGO = Instantiate(newShield.gameObject, ShieldAttachment.position, ShieldAttachment.rotation);
                CurrentShield = shieldGO.GetComponent<Shield>();
                shieldGO.transform.SetParent(ShieldAttachment);
                shieldGO.transform.localPosition = Vector3.zero;
                shieldGO.transform.localRotation = Quaternion.identity;

                // Initialize shield
                if (CurrentShield != null)
                {
                    CurrentShield.SetOwner(_character, this);
                    CurrentShield.Initialization();
                }
            }
        }

        protected override void HandleInput()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null) return;

            // Check for RMB on PC or Left Trigger on Controller
            var shieldButtonPressed =
                UnityEngine.Input.GetMouseButton(1);

            if (shieldButtonPressed)
            {
                if (!shieldActive) ShieldStart();
            }
            else if (shieldActive)
            {
                ShieldStop();
            }
        }


        public virtual void ShieldStart()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null) return;

            shieldActive = true;

            PlayAbilityStartFeedbacks();
            CurrentShield?.RaiseShield();
            if (_altCharacterHandleWeapon != null) _altCharacterHandleWeapon.enabled = false;
        }

        public virtual void ShieldStop()
        {
            if (!AbilityAuthorized || !InputAuthorized || CurrentShield == null) return;

            shieldActive = false;
            if (_altCharacterHandleWeapon != null) _altCharacterHandleWeapon.enabled = true;
            PlayAbilityStopFeedbacks();
            CurrentShield?.LowerShield();
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            ShieldStop();
        }

        protected override void OnRespawn()
        {
            base.OnRespawn();
            SetupShield();
        }
    }
}
