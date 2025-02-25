using Core.Events;
using Gameplay.Player.Stats;
using MoreMountains.Tools;
using ProgressionSystem.Scripts.Variables;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Character.Attributes.Dexterity
{
    public class PlayerDexterityManager : MonoBehaviour, IPlayerAttributeManager,
        MMEventListener<AttributeEvent>, MMEventListener<AttributeLevelEvent>
    {
        static int _playerDexterityLevel;
        static float _playerDexterityExperiencePoints;

        static int _initialDexterityLevel;
        static float _initialDexterityExperiencePoints;


        public LevelValueCurveVariable levelValueCurveVariable;


        [FormerlySerializedAs("dexterityBarUpdater")]
        public DexterityUIUpdater dexterityUIUpdater;


        string _savePath;

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
        public int GetAttributeValue()
        {
            return _playerDexterityLevel;
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
                _playerDexterityLevel = ES3.Load<int>("PlayerDexterityLevel", saveFilePath);
                _playerDexterityExperiencePoints = ES3.Load<float>("PlayerDexterityExperiencePoints", saveFilePath);
            }
            else
            {
                Debug.LogError("No save file found for player dexterity");
                ResetPlayerDexterity();
            }

            AttributeEvent.Trigger(
                AttributeInQuestion.Dexterity, AttributeEventType.Initialize,
                _playerDexterityExperiencePoints);

            AttributeLevelEvent.Trigger(
                AttributeInQuestion.Dexterity, AttributeLevelEventType.Initialize,
                _playerDexterityLevel);
        }
        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
        public void OnMMEvent(AttributeEvent eventType)
        {
            switch (eventType.EventType)
            {
                case AttributeEventType.Initialize:
                    Debug.Log("Initializing dexterity");
                    break;
                case AttributeEventType.IncreaseExperiencePoints:
                    AddExperiencePoints(eventType.ExperienceByValue);
                    break;
                case AttributeEventType.DecreaseExperiencePoints:
                    Debug.Log("Decreasing dexterity experience points");

                    break;
            }
        }
        public void OnMMEvent(AttributeLevelEvent eventType)
        {
            switch (eventType.EventType)
            {
                case AttributeLevelEventType.Initialize:
                    Debug.Log("Initializing dexterity level");
                    break;
                case AttributeLevelEventType.LevelUp:
                    AddAttributeLevel(eventType.Level);
                    Debug.Log("Leveling up dexterity");
                    break;
                case AttributeLevelEventType.Reset:
                    Debug.Log("Resetting dexterity level");
                    break;
            }
        }


        void AddExperiencePoints(float experiencePoints)
        {
            _playerDexterityExperiencePoints += experiencePoints;


            var newLevel =
                levelValueCurveVariable.GetLevelGivenExperience((int)_playerDexterityExperiencePoints);


            if (newLevel > _playerDexterityLevel)
            {
                AddAttributeLevel(newLevel - _playerDexterityLevel);
            }
            else
            {
                SavePlayerDexterity();
                Debug.Log("Saved dex: lvl: " + _playerDexterityLevel + " / exp: " + _playerDexterityExperiencePoints);
            }
        }


        static void AddAttributeLevel(int levels)
        {
            _playerDexterityLevel += levels;

            Debug.Log("Dexterity leveled up by: " + _playerDexterityLevel);


            SavePlayerDexterity();
        }

        static void SavePlayerDexterity()
        {
            ES3.Save("PlayerDexterityLevel", _playerDexterityLevel, GetSaveFilePath());
            ES3.Save("PlayerDexterityExperiencePoints", _playerDexterityExperiencePoints, GetSaveFilePath());
            Debug.Log(
                "Saved player dexterity: LVL: " + _playerDexterityLevel + " / PTS: " + _playerDexterityExperiencePoints
            );
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "PlayerDexterity.es3" : $"{slotPath}/PlayerDexterity.es3";
        }


        public static void ResetPlayerDexterity()
        {
            var characterStatProfile = PlayerAttributesProgressionManager.GetCharacterStatProfile();

            _initialDexterityLevel = characterStatProfile.InitialDexterityLevel;
            _initialDexterityExperiencePoints = characterStatProfile.InitialDexterityExperiencePoints;

            _playerDexterityLevel = characterStatProfile.InitialDexterityLevel;
            _playerDexterityExperiencePoints = characterStatProfile.InitialDexterityExperiencePoints;


            AttributeEvent.Trigger(
                AttributeInQuestion.Dexterity, AttributeEventType.Reset,
                _playerDexterityExperiencePoints);

            AttributeLevelEvent.Trigger(
                AttributeInQuestion.Dexterity, AttributeLevelEventType.Reset,
                _playerDexterityLevel);

            SavePlayerDexterity();
        }

        [Button(ButtonSizes.Medium)]
        public void IncreaseDexterityExperience(float experiencePoints)
        {
            AttributeEvent.Trigger(
                AttributeInQuestion.Dexterity, AttributeEventType.IncreaseExperiencePoints, experiencePoints);
        }
    }
}
