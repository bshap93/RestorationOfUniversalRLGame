using System;
using MoreMountains.Tools;

namespace Core.Events
{
    [Serializable]
    public enum StaminaEventType
    {
        ConsumeStamina,
        RecoverStamina,
        FullyRecoverStamina,
        IncreaseMaximumStamina,
        Initialize
    }

    public struct StaminaEvent
    {
        static StaminaEvent e;

        public StaminaEventType EventType;
        public float ByValue;

        public static void Trigger(StaminaEventType staminaEventType,
            float byValue)
        {
            e.EventType = staminaEventType;
            e.ByValue = byValue;
            MMEventManager.TriggerEvent(e);
        }
    }
}
