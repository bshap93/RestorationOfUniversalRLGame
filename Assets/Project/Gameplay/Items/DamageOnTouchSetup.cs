using MoreMountains.TopDownEngine;
using UnityEngine;

// 3. Verify DamageOnTouch settings
namespace Project.Gameplay.Combat
{
    public class DamageOnTouchSetup : MonoBehaviour
    {
        void VerifyDamageOnTouch()
        {
            var damageArea = GetComponent<DamageOnTouch>();

            // Enable all trigger callbacks for reliable detection
            damageArea.TriggerFilter = DamageOnTouch.AllowedTriggerCallbacks;

            // For debugging
            Debug.Log(
                $"DamageOnTouch Setup - TargetMask: {damageArea.TargetLayerMask.value}, " +
                $"MinDamage: {damageArea.MinDamageCaused}, MaxDamage: {damageArea.MaxDamageCaused}");
        }
    }
}
