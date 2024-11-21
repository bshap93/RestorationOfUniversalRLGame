using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    public enum WeaponAttachmentType
    {
        RightHandMelee,
        TwoHandedMelee,
        TwoHandedBow,
        RightHandRanged,
        TwoHandedRanged,
        Other
    }

    [AddComponentMenu("Roguelike/Character/Abilities/Alternate Character Handle Weapon")]
    public class AltCharacterHandleWeapon : CharacterHandleWeapon
    {
        protected new const string _weaponEquippedAnimationParameterName = "WeaponEquipped";
        protected new const string _weaponEquippedIDAnimationParameterName = "WeaponEquippedID";

        public WeaponAttachmentType WeaponAttachmentType;

        [Header("Weapon Attachment Points")] [Tooltip("List of attachment points based on weapon types.")]
        public List<AttachmentPoint> AttachmentPointList;

        [Tooltip("The position the weapon will be attached to. If left blank, will be this.transform.")]
        public new Transform WeaponAttachment;

        public WeaponIK WeaponIK;

        /// <summary>
        ///     Sets the weapon attachment based on WeaponAttachmentType.
        /// </summary>
        protected override void PreInitialization()
        {
            base.PreInitialization();

            WeaponAttachment = transform; // Default if no specific attachment is found
            foreach (var point in AttachmentPointList)
                if (point.Type == WeaponAttachmentType)
                {
                    WeaponAttachment = point.Attachment;
                    break;
                }

            Debug.Log($"WeaponAttachment set to {WeaponAttachment.name} for {WeaponAttachmentType}");
        }

        /// <summary>
        ///     Instantiates the specified weapon.
        /// </summary>
        protected override void InstantiateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (WeaponAttachment == null) PreInitialization();

            if (!combo)
                CurrentWeapon = Instantiate(
                    newWeapon,
                    WeaponAttachment.position + newWeapon.WeaponAttachmentOffset,
                    WeaponAttachment.rotation);

            CurrentWeapon.name = newWeapon.name;
            CurrentWeapon.transform.parent = WeaponAttachment;
            CurrentWeapon.transform.localPosition = newWeapon.WeaponAttachmentOffset;
            CurrentWeapon.SetOwner(_character, this);
            CurrentWeapon.WeaponID = weaponID;
            CurrentWeapon.FlipWeapon();

            _weaponAim = CurrentWeapon.gameObject.MMGetComponentNoAlloc<WeaponAim>();
            HandleWeaponAim();
            HandleWeaponIK();
            HandleWeaponModel(newWeapon, weaponID, combo, CurrentWeapon);

            CurrentWeapon.Initialization();
            CurrentWeapon.InitializeComboWeapons();
            CurrentWeapon.InitializeAnimatorParameters();
            InitializeAnimatorParameters();
        }

        public override void ChangeWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (CurrentWeapon != null)
            {
                CurrentWeapon.TurnWeaponOff();

                if (!combo)
                {
                    ShootStop();

                    if (_weaponAim != null) _weaponAim.RemoveReticle();

                    if (_character._animator != null)
                    {
                        var parameters = _character._animator.parameters;
                        foreach (var parameter in parameters)
                            if (parameter.name == CurrentWeapon.EquippedAnimationParameter)
                                MMAnimatorExtensions.UpdateAnimatorBool(
                                    _animator, CurrentWeapon.EquippedAnimationParameter, false);
                    }

                    Destroy(CurrentWeapon.gameObject);
                }
            }

            if (newWeapon != null)
            {
                // Fetch WeaponAttachmentType from the WeaponAttachmentTypeComponent
                var attachmentTypeComponent = newWeapon.GetComponent<WeaponAttachmentTypeComponent>();
                WeaponAttachmentType = attachmentTypeComponent != null
                    ? attachmentTypeComponent.AttachmentType
                    : WeaponAttachmentType.Other;

                var useWeaponIK = attachmentTypeComponent != null && attachmentTypeComponent.UseWeaponIK;


                // Toggle WeaponIK based on WeaponAttachmentTypeComponent
                ToggleWeaponIK(useWeaponIK);

                // Set WeaponAttachment before instantiating the weapon
                SetWeaponAttachment();

                InstantiateWeapon(newWeapon, weaponID, combo);
            }
            else
            {
                CurrentWeapon = null;
                HandleWeaponModel(null, null);


                // Toggle WeaponIK off if no weapon is equipped
                ToggleWeaponIK(false);
            }

            OnWeaponChange?.Invoke();
        }


        void SetWeaponAttachment()
        {
            WeaponAttachment = transform; // Default to the character's transform

            foreach (var point in AttachmentPointList)
                if (point.Type == WeaponAttachmentType)
                {
                    WeaponAttachment = point.Attachment;
                    break;
                }

            Debug.Log($"WeaponAttachment set to {WeaponAttachment.name} for {WeaponAttachmentType}");
        }

        void ToggleWeaponIK(bool enable)
        {
            // Find the child object containing WeaponIK
            if (WeaponIK != null)
            {
                WeaponIK.enabled = enable;
                Debug.Log($"WeaponIK {(enable ? "enabled" : "disabled")}");
            }
            else
            {
                Debug.LogWarning("WeaponIKObject not found. Please ensure the hierarchy is correct.");
            }
        }

        [Serializable]
        public struct AttachmentPoint
        {
            public WeaponAttachmentType Type;
            public Transform Attachment;
        }
    }
}
