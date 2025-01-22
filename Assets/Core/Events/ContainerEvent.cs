namespace Project.Core.Events
{
    public enum ContainerEventType
    {
        ContainerInRange,
        ContainerOutOfRange,
        ContainerSelected,
        ContainerDeselected,

    }
    public class ContainerEvent
    {
        static ContainerEvent e;
        
        public string EventName;
        public ContainerEventType EventType;
        public ContainerController ContainerControllerParameter;
        
        
    }
}
