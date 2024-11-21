using MoreMountains.TopDownEngine;
using Project.Gameplay.Magic;
using UnityEngine;

namespace Project.Gameplay.Combat.Abilities
{
    [AddComponentMenu("TopDown Engine/Magic/MagicAbility")]
    public abstract class MagicAbility : CharacterAbility
    {
        [Header("Magic Ability Settings")] public float CooldownDuration = 1f; // Time between uses
        public float KinemaCost = 10f; // Resource cost
        public float FavourCost = 10f; // Resource cost
        public MagicSystem MagicSystem; // Reference to the character's MagicSystem

        float _lastUseTime;

        protected override void Initialization()
        {
            base.Initialization();
            _lastUseTime = -CooldownDuration; // Allow immediate use
        }

        public override void ProcessAbility()
        {
            base.ProcessAbility();

            // Optional: Add continuous checks or updates here
        }

        protected bool CanCast()
        {
            return Time.time >= _lastUseTime + CooldownDuration
                   && MagicSystem != null
                   && MagicSystem.CanConsumePrimary(KinemaCost) && MagicSystem.CanConsumeSecondary(FavourCost);
        }

        protected void ConsumeResource()
        {
            if (MagicSystem != null)
            {
                MagicSystem.ConsumePrimary(KinemaCost);
                MagicSystem.ConsumeSecondary(FavourCost);
            }
        }

        protected void StartCooldown()
        {
            _lastUseTime = Time.time;
        }

        public abstract void Cast();
    }
}
