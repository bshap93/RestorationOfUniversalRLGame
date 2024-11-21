using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Combat.Weapons;
using UnityEngine;

namespace Project.Gameplay.AI
{
    [AddComponentMenu("TopDown Engine/Character/AI/Actions/AI Action Melee")]
    public class AIActionMelee : AIAction
    {
        [Header("Attack Settings")] [Tooltip("Minimum time between attacks")]
        public float MinTimeBetweenAttacks = 1f;

        [Tooltip("Maximum time between attacks")]
        public float MaxTimeBetweenAttacks = 2f;

        [Tooltip("The minimum distance at which the character can attack")]
        public float MinimumAttackDistance = 1.5f;

        [Header("Weapon Handling")] [Tooltip("The CharacterHandleWeapon ability this AI action should pilot")]
        public AltCharacterHandleWeapon TargetHandleWeaponAbility;
        protected bool _attackInProgress;
        protected Character _character;

        protected float _lastAttackTime = -1000f;
        protected float _nextAttackTime;
        protected CharacterOrientation3D _orientation3D;

        public override void Initialization()
        {
            if (!ShouldInitialize) return;
            base.Initialization();

            _character = GetComponentInParent<Character>();
            _orientation3D = _character?.FindAbility<CharacterOrientation3D>();

            if (TargetHandleWeaponAbility == null)
                TargetHandleWeaponAbility = _character?.FindAbility<AltCharacterHandleWeapon>();

            // Set initial next attack time
            SetNextAttackTime();
        }

        public override void OnEnterState()
        {
            base.OnEnterState();
            _attackInProgress = false;
            SetNextAttackTime();
        }

        public override void PerformAction()
        {
            // If no target or weapon, we exit
            if (_brain.Target == null || TargetHandleWeaponAbility?.CurrentWeapon == null) return;

            // Calculate distance to target
            var distanceToTarget = Vector3.Distance(_character.transform.position, _brain.Target.position);

            // If we're too far, don't attack
            if (distanceToTarget > MinimumAttackDistance) return;

            // Check if we can attack based on timing
            if (Time.time >= _nextAttackTime && !_attackInProgress) StartAttack();

            // Check if current attack is complete
            if (_attackInProgress && TargetHandleWeaponAbility.CurrentWeapon.WeaponState.CurrentState ==
                Weapon.WeaponStates.WeaponIdle)
            {
                _attackInProgress = false;
                SetNextAttackTime();
            }
        }

        protected virtual void StartAttack()
        {
            _lastAttackTime = Time.time;
            _attackInProgress = true;
            TargetHandleWeaponAbility.ShootStart();
        }

        protected virtual void SetNextAttackTime()
        {
            _nextAttackTime = Time.time + Random.Range(MinTimeBetweenAttacks, MaxTimeBetweenAttacks);
        }

        public override void OnExitState()
        {
            base.OnExitState();

            if (TargetHandleWeaponAbility != null) TargetHandleWeaponAbility.ForceStop();

            _attackInProgress = false;
        }
    }
}
