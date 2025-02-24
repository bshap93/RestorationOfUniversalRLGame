using Core.Events;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class PickaxeTriggrerableEffects : MonoBehaviour
    {
        public float StaminaCost = 5;
        public void TriggerEffects()
        {
            StaminaEvent.Trigger(StaminaEventType.ConsumeStamina, StaminaCost);
            Debug.Log("Triggering pickaxe effects");
        }
    }
}
