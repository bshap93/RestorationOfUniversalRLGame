using Core.Events;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Gameplay.Character.Attributes.Dexterity
{
    public class DexterityUIUpdater : MonoBehaviour,
        MMEventListener<AttributeEvent>, MMEventListener<AttributeLevelEvent>
    {
        public TMP_Text lvl;
        public TMP_Text exp;
        float _currentDexterityExperiencePoints;

        float _currentDexterityLevel;

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
            if (eventType.AttributeInQuestion == AttributeInQuestion.Dexterity)
                switch (eventType.EventType)
                {
                    case AttributeEventType.Initialize:
                        _currentDexterityExperiencePoints = eventType.ExperienceByValue;
                        exp.text = _currentDexterityExperiencePoints.ToString();

                        break;
                    case AttributeEventType.IncreaseExperiencePoints:
                        _currentDexterityExperiencePoints += eventType.ExperienceByValue;
                        exp.text = _currentDexterityExperiencePoints.ToString();
                        break;

                    case AttributeEventType.Reset:
                        _currentDexterityExperiencePoints = 0;
                        exp.text = _currentDexterityExperiencePoints.ToString();

                        break;
                }
        }

        public void OnMMEvent(AttributeLevelEvent eventType)
        {
            if (eventType.AttributeInQuestion == AttributeInQuestion.Dexterity)
                switch (eventType.EventType)
                {
                    case AttributeLevelEventType.Initialize:
                        _currentDexterityLevel = eventType.Level;
                        lvl.text = _currentDexterityLevel.ToString();
                        break;
                    case AttributeLevelEventType.LevelUp:
                        _currentDexterityLevel = eventType.Level;
                        lvl.text = _currentDexterityLevel.ToString();
                        break;
                    case AttributeLevelEventType.Reset:
                        _currentDexterityLevel = 0;
                        lvl.text = _currentDexterityLevel.ToString();
                        break;
                }
        }
    }
}
