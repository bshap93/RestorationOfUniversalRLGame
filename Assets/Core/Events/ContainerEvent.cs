using Gameplay.ItemsInteractions.Containers;
using MoreMountains.Tools;

namespace Core.Events
{
    public enum ContainerEventType
    {
        ContainerInRange,
        ContainerOutOfRange,
        ContainerSelected,
        ContainerDeselected
    }

    public struct ContainerEvent
    {
        static ContainerEvent e;

        public string EventName;
        public ContainerEventType EventType;
        public ContainerController ContainerControllerParameter;

        public static void Trigger(string eventName, ContainerEventType containerEventType,
            ContainerController containerController)
        {
            e.EventName = eventName;
            e.EventType = containerEventType;
            e.ContainerControllerParameter = containerController;
            MMEventManager.TriggerEvent(e);
        }
    }
}
