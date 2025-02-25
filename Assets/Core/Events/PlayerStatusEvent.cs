using System;
using MoreMountains.Tools;

namespace Core.Events
{
    [Serializable]
    public enum PlayerStatusEventType
    {
        OutOfStamina,
        RegainedStamina,
        OutOfHealth,
        RegainedHealth,
        ImmuneToDamage
    }

    public struct PlayerStatusEvent
    {
        static PlayerStatusEvent e;

        public PlayerStatusEventType EventType;

        public static void Trigger(PlayerStatusEventType eventType)
        {
            e.EventType = eventType;
            MMEventManager.TriggerEvent(e);
        }
    }
}
