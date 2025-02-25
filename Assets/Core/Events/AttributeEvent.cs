using System;
using MoreMountains.Tools;

namespace Core.Events
{
    [Serializable]
    public enum AttributeInQuestion
    {
        Dexterity,
        Endurance
    }

    [Serializable]
    public enum AttributeEventType
    {
        IncreaseExperiencePoints,
        DecreaseExperiencePoints,
        Initialize,
        SetAttributeLevel,
        Reset
    }

    [Serializable]
    public enum AttributeLevelEventType
    {
        LevelUp,
        Reset,
        Initialize
    }

    /// <summary>
    ///     Used to change the experience of an attribute
    /// </summary>
    public struct AttributeEvent
    {
        static AttributeEvent _e;

        public AttributeInQuestion AttributeInQuestion;
        public AttributeEventType EventType;
        public float ExperienceByValue;

        public static void Trigger(AttributeInQuestion attributeInQuestion,
            AttributeEventType attributeEventType, float experienceByValue)
        {
            _e.AttributeInQuestion = attributeInQuestion;
            _e.EventType = attributeEventType;
            _e.ExperienceByValue = experienceByValue;

            MMEventManager.TriggerEvent(_e);
        }
    }

    public struct AttributeLevelEvent
    {
        static AttributeLevelEvent _e;

        public AttributeInQuestion AttributeInQuestion;
        public AttributeLevelEventType EventType;
        public int Level;

        public static void Trigger(AttributeInQuestion attributeInQuestion,
            AttributeLevelEventType attributeEventType, int level)
        {
            _e.AttributeInQuestion = attributeInQuestion;
            _e.EventType = attributeEventType;
            _e.Level = level;

            MMEventManager.TriggerEvent(_e);
        }
    }
}
