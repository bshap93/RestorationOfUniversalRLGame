using Core.Events;
using MoreMountains.Tools;
using ProgressionSystem.Scripts.Variables;
using UnityEngine;

namespace Gameplay.Character.Attributes.Dexterity
{
    public class PlayerDexterityManager : MonoBehaviour, IPlayerAttributeManager,
        MMEventListener<AttributeExperienceEvent>
    {
        public static int PlayerDexterityLevel;
        public static float PlayerDexterityExperiencePoints;

        static int _initialDexterityLevel;
        static float _initialDexterityExperiencePoints;


        public LevelValueCurveVariable levelValueCurveVariable;


        public DexterityBarUpdater dexterityBarUpdater;


        string _savePath;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
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
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(saveFilePath);
            if (exists)
            {
                PlayerDexterityLevel = ES3.Load<int>("PlayerDexterityLevel", saveFilePath);
                PlayerDexterityExperiencePoints = ES3.Load<float>("PlayerDexterityExperiencePoints", saveFilePath);
                dexterityBarUpdater.Initialize();
            }
            else
            {
                Debug.LogError("No save file found for player dexterity");
                ResetPlayerDexterity();
                dexterityBarUpdater.Initialize();
            }
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
        public void OnMMEvent(AttributeExperienceEvent eventType)
        {
            switch (eventType.EventType)
            {
                case AttributeEventType.Initialize:
                    Debug.Log("Initializing dexterity");
                    break;
                case AttributeEventType.IncreaseExperiencePoints:
                    AddExperiencePoints(eventType.ByValue);
                    break;
                case AttributeEventType.DecreaseExperiencePoints:
                    Debug.Log("Decreasing dexterity experience points");

                    break;
            }
        }


        public void AddExperiencePoints(int experiencePoints)
        {
            PlayerDexterityExperiencePoints += experiencePoints;

            var newLevel =
                levelValueCurveVariable.GetLevelGivenExperience((int)PlayerDexterityExperiencePoints);


            if (newLevel > PlayerDexterityLevel)
            {
                AddAttributeLevel(newLevel - PlayerDexterityLevel);
            }
            else
            {
                SavePlayerDexterity();
                Debug.Log("Saved dex: lvl: " + PlayerDexterityLevel + " / exp: " + PlayerDexterityExperiencePoints);
            }
        }

        public static void SetAttributeLevel(int experiencePoints)
        {
            PlayerDexterityLevel = experiencePoints;
            SavePlayerDexterity();
            Debug.Log("Saved dex: lvl: " + PlayerDexterityLevel + " / exp: " + PlayerDexterityExperiencePoints);
        }

        static void AddAttributeLevel(int levels)
        {
            PlayerDexterityLevel += levels;
            SavePlayerDexterity();
        }

        public static void SavePlayerDexterity()
        {
            ES3.Save("PlayerDexterityLevel", PlayerDexterityLevel, GetSaveFilePath());
            ES3.Save("PlayerDexterityExperiencePoints", PlayerDexterityExperiencePoints, GetSaveFilePath());
            Debug.Log(
                "Saved player dexterity: LVL: " + PlayerDexterityLevel + " / PTS: " + PlayerDexterityExperiencePoints
            );
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

            PlayerDexterityLevel = _initialDexterityLevel;
            PlayerDexterityExperiencePoints = _initialDexterityExperiencePoints;
        }


        public static void ResetPlayerDexterity()
        {
            PlayerDexterityLevel = _initialDexterityLevel;
            PlayerDexterityExperiencePoints = _initialDexterityExperiencePoints;

            SavePlayerDexterity();
        }
    }
}
