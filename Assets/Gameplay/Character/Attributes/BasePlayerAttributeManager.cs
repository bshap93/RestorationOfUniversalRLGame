using Core.Events;
using Gameplay.Character.Attributes.LevelExperienceCurve;
using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Gameplay.Character.Attributes
{
    public abstract class BasePlayerAttributeManager : MonoBehaviour, IPlayerAttributeManager,
        MMEventListener<AttributeEvent>, MMEventListener<AttributeLevelEvent>
    {
        // Config
        public LevelValueCurveVariable levelValueCurveVariable;
        protected float attributeExperiencePoints;

        protected int attributeLevel;
        protected string savePath;


        protected abstract AttributeInQuestion AttributeType { get; }
        protected abstract string SaveFileName { get; }


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

        public int GetAttributeLevelValue()
        {
            return attributeLevel;
        }

        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Start()
        {
            savePath = GetSaveFilePath();
            LoadPlayerAttribute();
        }

        public void LoadPlayerAttribute()
        {
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(saveFilePath);
            if (exists)
            {
                attributeLevel = ES3.Load("Player" + AttributeType + "Level", saveFilePath, 0);
                attributeExperiencePoints = ES3.Load("Player" + AttributeType + "ExperiencePoints", saveFilePath, 0f);
            }
            else
            {
                Debug.Log("No save file found for " + AttributeType);
                ResetAttribute();
            }

            AttributeEvent.Trigger(AttributeType, AttributeEventType.Initialize, attributeExperiencePoints);
            AttributeLevelEvent.Trigger(AttributeType, AttributeLevelEventType.Initialize, attributeLevel);
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
        public void OnMMEvent(AttributeEvent eventType)
        {
            // Only handle events for this attribute type
            if (eventType.AttributeInQuestion != AttributeType) return;

            switch (eventType.EventType)
            {
                case AttributeEventType.Initialize:
                    Debug.Log($"Initializing {AttributeType}");
                    break;
                case AttributeEventType.IncreaseExperiencePoints:
                    AddExperiencePoints(eventType.ExperienceByValue);
                    break;
                case AttributeEventType.DecreaseExperiencePoints:
                    Debug.Log($"Decreasing {AttributeType} experience points");
                    break;
            }
        }
        public void OnMMEvent(AttributeLevelEvent eventType)
        {
            // Only handle events for this attribute type
            if (eventType.AttributeInQuestion != AttributeType) return;

            switch (eventType.EventType)
            {
                case AttributeLevelEventType.Initialize:
                    Debug.Log($"Initializing {AttributeType} level");
                    break;
                case AttributeLevelEventType.Reset:
                    Debug.Log($"Resetting {AttributeType} level");
                    break;
            }
        }

        [Button(ButtonSizes.Medium)]
        public void IncreaseExperience(float experiencePoints)
        {
            AttributeEvent.Trigger(
                AttributeType, AttributeEventType.IncreaseExperiencePoints, experiencePoints);
        }

        public abstract void ResetAttribute();

        protected string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? $"{SaveFileName}.es3" : $"{slotPath}/{SaveFileName}.es3";
        }

        void AddExperiencePoints(float experiencePoints)
        {
            attributeExperiencePoints += experiencePoints;


            var newLevel =
                levelValueCurveVariable.GetLevelGivenExperience((int)attributeExperiencePoints);


            if (newLevel > attributeLevel)
                AddAttributeLevel(newLevel - attributeLevel);
            else
                Debug.Log(
                    "Saved " + AttributeType + ": lvl: " + attributeLevel + " / exp: " + attributeExperiencePoints);

            SaveAttribute();
        }
        protected void SaveAttribute()
        {
            ES3.Save($"Player{AttributeType}Level", attributeLevel, GetSaveFilePath());
            ES3.Save($"Player{AttributeType}ExperiencePoints", attributeExperiencePoints, GetSaveFilePath());
            Debug.Log(
                $"Saved player {AttributeType}: LVL: {attributeLevel} / PTS: {attributeExperiencePoints}"
            );
        }

        protected void AddAttributeLevel(int levels)
        {
            attributeLevel += levels;

            AttributeLevelEvent.Trigger(
                AttributeType, AttributeLevelEventType.LevelUp,
                attributeLevel);

            Debug.Log($"{AttributeType} leveled up by: {levels} to {attributeLevel}");
        }
    }
}
