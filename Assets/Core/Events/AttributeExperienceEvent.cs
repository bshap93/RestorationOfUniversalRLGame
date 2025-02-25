using System;
using MoreMountains.Tools;

namespace Core.Events
{
    [Serializable]
    public enum AttributeInQuestion
    {
        Dexterity
    }

    [Serializable]
    public enum AttributeEventType
    {
        IncreaseExperiencePoints,
        DecreaseExperiencePoints,
        Initialize
    }

    [Serializable]
    public enum AttributeLevelEventType
    {
        IncreaseLevel,
        DecreaseLevel,
        Initialize
    }

    /// <summary>
    ///     Used to change the experience of an attribute
    /// </summary>
    public struct AttributeExperienceEvent
    {
        static AttributeExperienceEvent _e;

        public AttributeInQuestion AttributeInQuestion;
        public AttributeEventType EventType;
        public int ByValue;

        public static void Trigger(AttributeInQuestion attributeInQuestion,
            AttributeEventType attributeEventType, int byValue)
        {
            _e.AttributeInQuestion = attributeInQuestion;
            _e.EventType = attributeEventType;
            _e.ByValue = byValue;
            MMEventManager.TriggerEvent(_e);
        }
    }

    /// <summary>
    ///     Used to change the level of an attribute
    /// </summary>
    public struct AttributeLevelEvent
    {
        static AttributeLevelEvent _e;

        public AttributeLevelEventType EventType;
        public AttributeInQuestion AttributeInQuestion;
        public int ByValue;

        public static void Trigger(AttributeInQuestion attributeInQuestion,
            AttributeLevelEventType attributeEventType, int byValue)
        {
            _e.AttributeInQuestion = attributeInQuestion;
            _e.EventType = attributeEventType;
            _e.ByValue = byValue;
            MMEventManager.TriggerEvent(_e);
        }
    }
}
