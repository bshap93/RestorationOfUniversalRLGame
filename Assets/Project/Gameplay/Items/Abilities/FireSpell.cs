using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat.Abilities
{
    public class FireSpell : MagicAbility
    {
        public ProjectileWeapon ProjectileWeapon; // Reference to the projectile weapon for the fire spell

        public override void Cast()
        {
            if (CanCast())
            {
                // Consume resource and start cooldown
                ConsumeResource();
                StartCooldown();

                // Trigger the weapon's shooting mechanism
                if (ProjectileWeapon != null)
                {
                    ProjectileWeapon.WeaponUse();
                }
            }
            else
            {
                Debug.Log("Cannot cast Fire Spell: Not enough Kinema or still on cooldown.");
            }
        }
    }
}
