using System;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat.Weapons
{
    public enum WeaponAttachmentType
    {
        RightHandMelee,
        TwoHandedMelee,
        TwoHandedBow,
        RightHandRanged,
        TwoHandedRanged,
        TwoHandedSpearlike,
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

        [FormerlySerializedAs("WeaponAttachment")]
        [Tooltip("The position the weapon will be attached to. If left blank, will be this.transform.")]
        public Transform weaponAttachment;

        public WeaponIK WeaponIK;

        [Header("Weapon Equip Feedback")] [Tooltip("Feedback to play when a weapon is equipped")]
        public MMFeedbacks WeaponEquipFeedback;


        /// <summary>
        ///     Sets the weapon attachment based on WeaponAttachmentType.
        /// </summary>
        protected override void PreInitialization()
        {
            base.PreInitialization();

            weaponAttachment = transform; // Default if no specific attachment is found
            foreach (var point in AttachmentPointList)
                if (point.Type == WeaponAttachmentType)
                {
                    weaponAttachment = point.Attachment;
                    break;
                }
        }

        /// <summary>
        ///     Instantiates the specified weapon.
        /// </summary>
        protected override void InstantiateWeapon(Weapon newWeapon, string weaponID, bool combo = false)
        {
            if (weaponAttachment == null) PreInitialization();

            if (!combo)
                CurrentWeapon = Instantiate(
                    newWeapon,
                    weaponAttachment.position + newWeapon.WeaponAttachmentOffset,
                    weaponAttachment.rotation);

            CurrentWeapon.name = newWeapon.name;
            CurrentWeapon.transform.parent = weaponAttachment;
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
            WeaponEquipFeedback?.Initialization(gameObject);
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
                    Destroy(CurrentWeapon.gameObject);
                }
            }

            if (newWeapon != null)
            {
                var attachmentTypeComponent = newWeapon.GetComponent<WeaponAttachmentTypeComponent>();
                WeaponAttachmentType = attachmentTypeComponent != null
                    ? attachmentTypeComponent.AttachmentType
                    : WeaponAttachmentType.Other;

                ToggleWeaponIK(attachmentTypeComponent?.UseWeaponIK ?? false);
                SetWeaponAttachment();

                InstantiateWeapon(newWeapon, weaponID, combo);
                WeaponEquipFeedback?.PlayFeedbacks();
            }
            else
            {
                CurrentWeapon = null;
                HandleWeaponModel(null, null);
                ToggleWeaponIK(false);
            }

            OnWeaponChange?.Invoke();
        }


        void SetWeaponAttachment()
        {
            weaponAttachment = transform; // Default to the character's transform

            foreach (var point in AttachmentPointList)
                if (point.Type == WeaponAttachmentType)
                {
                    weaponAttachment = point.Attachment;
                    break;
                }
        }

        void ToggleWeaponIK(bool enable)
        {
            // Find the child object containing WeaponIK
            if (WeaponIK != null) WeaponIK.enabled = enable;
        }

        [Serializable]
        public struct AttachmentPoint
        {
            public WeaponAttachmentType Type;
            public Transform Attachment;
        }
    }
}
