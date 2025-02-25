using Core.Events;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Gameplay.Character.Attributes.Endurance
{
    public abstract class AttributeUIUpdater : MonoBehaviour,
        MMEventListener<AttributeEvent>, MMEventListener<AttributeLevelEvent>
    {
        public TMP_Text header;
        public TMP_Text lvl;
        public TMP_Text exp;

        protected float CurrentAttributeExperiencePoints;

        protected float CurrentAttributeLevel;

        protected abstract AttributeInQuestion AttributeType { get; }

        void OnEnable()
        {
            this.MMEventStartListening<AttributeEvent>();
            this.MMEventStartListening<AttributeLevelEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<AttributeEvent>();
            this.MMEventStopListening<AttributeLevelEvent>();
        }


        public void OnMMEvent(AttributeEvent eventType)
        {
            if (eventType.AttributeInQuestion == AttributeType)
                switch (eventType.EventType)
                {
                    case AttributeEventType.Initialize:
                        CurrentAttributeExperiencePoints = eventType.ExperienceByValue;
                        exp.text = CurrentAttributeExperiencePoints.ToString();
                        header.text = AttributeType.ToString();
                        break;
                    case AttributeEventType.IncreaseExperiencePoints:
                        CurrentAttributeExperiencePoints += eventType.ExperienceByValue;
                        exp.text = CurrentAttributeExperiencePoints.ToString();
                        break;
                    case AttributeEventType.Reset:
                        CurrentAttributeExperiencePoints = 0;
                        exp.text = CurrentAttributeExperiencePoints.ToString();
                        break;
                }
        }
        public void OnMMEvent(AttributeLevelEvent eventType)
        {
            if (eventType.AttributeInQuestion == AttributeType)
                switch (eventType.EventType)
                {
                    case AttributeLevelEventType.Initialize:
                        CurrentAttributeLevel = eventType.Level;

                        lvl.text = CurrentAttributeLevel.ToString();
                        break;
                    case AttributeLevelEventType.LevelUp:
                        CurrentAttributeLevel = eventType.Level;
                        lvl.text = CurrentAttributeLevel.ToString();
                        break;
                    case AttributeLevelEventType.Reset:
                        CurrentAttributeLevel = 0;
                        lvl.text = CurrentAttributeLevel.ToString();
                        break;
                }
        }
    }
}
