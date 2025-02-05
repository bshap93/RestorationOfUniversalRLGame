using MoreMountains.Tools;
using PixelCrushers.QuestMachine.Wrappers;

namespace Core.Events
{
    public enum QuestEventType
    {
        ForcePlayerToAcceptQuest
    }

    public struct MMQuestEvent
    {
        static MMQuestEvent e;

        public Quest QuestParameter;
        public QuestEventType EventType;

        public static void Trigger(QuestEventType questEventType, Quest quest)
        {
            e.EventType = questEventType;
            e.QuestParameter = quest;

            MMEventManager.TriggerEvent(e);
        }
    }
}
