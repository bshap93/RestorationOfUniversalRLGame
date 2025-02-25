using System;
using Core.Events;
using MoreMountains.Tools;
using UnityEngine;

namespace Gameplay.Character.Attributes.Dexterity
{
    public class PlayerDexterityManager : MonoBehaviour, IPlayerAttributeManager,
        MMEventListener<AttributeLevelEvent>, MMEventListener<AttributeExperienceEvent>
    {
        public static int PlayerDexterityLevel;
        public static int PlayerDexterityExperiencePoints;
        public static int PlayerDexterityExperiencePointsToNextLevel;

        static int _initialDexterityLevel;
        static int _initialDexterityExperiencePoints;
        static int _initialDexterityExperiencePointsToNextLevel;


        public DexterityBarUpdater dexterityBarUpdater;


        string _savePath;

        void OnEnable()
        {
            this.MMEventStartListening<AttributeLevelEvent>();
            this.MMEventStartListening<AttributeExperienceEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<AttributeLevelEvent>();
            this.MMEventStopListening<AttributeExperienceEvent>();
        }
        public int GetAttributeValue()
        {
            return PlayerDexterityLevel;
        }
        public void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        public void Start()
        {
            _savePath = GetSaveFilePath();
            LoadPlayerAttribute();
        }
        public void LoadPlayerAttribute()
        {
            throw new NotImplementedException();
        }
        public bool HasSavedData()
        {
            throw new NotImplementedException();
        }
        public void OnMMEvent(AttributeExperienceEvent eventType)
        {
            switch (eventType.EventType)
            {
            }
        }
        public void OnMMEvent(AttributeLevelEvent eventType)
        {
            switch (eventType.EventType)
            {
            }
        }

        public static void AddExperiencePoints(int experiencePoints)
        {
            PlayerDexterityExperiencePoints += experiencePoints;
            if (experiencePoints > PlayerDexterityExperiencePointsToNextLevel) 
            {
                SetAttributeLevel(PlayerDexterityExperiencePoints);
                
            }

            SavePlayerDexterity();
        }

        public static void SetAttributeLevel(int experiencePoints)
        {
            
        }

        public static void AddAttributeLevel(int levels)
        {
            PlayerDexterityLevel += levels;
            SavePlayerDexterity();
        }

        public static void SavePlayerDexterity()
        {
            ES3.Save("PlayerDexterityLevel", PlayerDexterityLevel, GetSaveFilePath());
            ES3.Save("PlayerDexterityExperiencePoints", PlayerDexterityExperiencePoints, GetSaveFilePath());
            ES3.Save(
                "PlayerDexterityExperiencePointsToNextLevel", PlayerDexterityExperiencePointsToNextLevel,
                GetSaveFilePath());

            Debug.Log(
                "Saved player dexterity: LVL: " + PlayerDexterityLevel + " / PTS: " + PlayerDexterityExperiencePoints +
                " / PTS_NEXT_LEVEL" + PlayerDexterityExperiencePointsToNextLevel);
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "PlayerDexterity.es3" : $"{slotPath}/PlayerDexterity.es3";
        }
        public static void Initialize(CharacterStatProfile characterStatProfile)
        {
            _initialDexterityLevel = characterStatProfile.InitialDexterityLevel;
            _initialDexterityExperiencePoints = characterStatProfile.InitialDexterityExperiencePoints;
            _initialDexterityExperiencePointsToNextLevel =
                characterStatProfile.InitialDexterityExperiencePointsToNextLevel;

            PlayerDexterityLevel = _initialDexterityLevel;
            PlayerDexterityExperiencePoints = _initialDexterityExperiencePoints;
            PlayerDexterityExperiencePointsToNextLevel = _initialDexterityExperiencePointsToNextLevel;
        }


        public static void ResetPlayerDexterity()
        {
            PlayerDexterityLevel = _initialDexterityLevel;
            PlayerDexterityExperiencePoints = _initialDexterityExperiencePoints;
            PlayerDexterityExperiencePointsToNextLevel = _initialDexterityExperiencePointsToNextLevel;

            SavePlayerDexterity();
        }
    }
}
