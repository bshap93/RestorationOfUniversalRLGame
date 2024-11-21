using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Combat
{
    public class MeleeWeaponSetup : MonoBehaviour
    {
        void SetupMeleeWeapon()
        {
            var weapon = GetComponent<MeleeWeapon>();

            // For a player weapon, target enemies
            weapon.TargetLayerMask = LayerMask.GetMask("Enemies");

            // For an enemy weapon, target the player
            // weapon.TargetLayerMask = LayerMask.GetMask("Player");

            // Configure damage area
            weapon.DamageAreaShape = MeleeWeapon.MeleeDamageAreaShapes.Rectangle; // or whatever shape you prefer
            weapon.AreaSize = new Vector3(2f, 2f, 2f); // Adjust size as needed
            weapon.MinDamageCaused = 10f;
            weapon.MaxDamageCaused = 10f;

            // Make sure damage detection is enabled
            weapon.InitialDelay = 0f;
            weapon.ActiveDuration = 0.5f;
        }
    }
}
