#if UNITY_EDITOR

using Core.Events;
using Gameplay.Extensions.TopDownEngineExtensions.Stamina;
using MoreMountains.Tools;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Player.Stats
{
    public static class PlayerMutableStatsManagerDebug
    {
        [MenuItem("Debug/Reset Stamina")]
        public static void ResetStamina()
        {
            PlayerStaminaManager.ResetPlayerStamina();
        }
    }
#endif
    public class PlayerStaminaManager : MonoBehaviour, MMEventListener<StaminaEvent>
    {
        public static float StaminaPoints;
        public static float MaxStaminaPoints;
        public static float InitialCharacterStamina = 100;
        public StaminaBarUpdater staminaBarUpdater;


        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _savePath = GetSaveFilePath();

            Initialize();
            // if (HasSavedData())
            //     LoadPlayerStamina();
            // else
            //     Initialize();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(StaminaEvent staminaEvent)
        {
            switch (staminaEvent.EventType)
            {
                case StaminaEventType.ConsumeStamina:
                    ConsumeStamina(staminaEvent.ByValue);
                    break;
                case StaminaEventType.RecoverStamina:
                    RecoverStamina(staminaEvent.ByValue);
                    break;
                case StaminaEventType.FullyRecoverStamina:
                    FullyRecoverStamina();
                    break;
                case StaminaEventType.IncreaseMaximumStamina:
                    IncreaseMaximumStamina(staminaEvent.ByValue);
                    break;
            }
        }

        public void Initialize()
        {
            Debug.LogError("Initializing player stamina");
            ResetPlayerStamina();
            staminaBarUpdater.Initialize();
        }

        public static void ConsumeStamina(float amount)
        {
            StaminaPoints -= amount;
        }

        public static void RecoverStamina(float amount)
        {
            StaminaPoints += amount;
        }

        public static void FullyRecoverStamina()
        {
            StaminaPoints = MaxStaminaPoints;
            SavePlayerStamina();
        }

        public static void IncreaseMaximumStamina(float amount)
        {
            MaxStaminaPoints += amount;
            SavePlayerStamina();
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "PlayerStamina.es3" : $"{slotPath}/PlayerStamina.es3";
        }

        public void LoadPlayerStamina()
        {
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(_savePath);
            if (exists)
            {
                StaminaPoints = ES3.Load<float>("StaminaPoints", _savePath);
                MaxStaminaPoints = ES3.Load<float>("MaxStaminaPoints", _savePath);
            }
        }
        public static void ResetPlayerStamina()
        {
            StaminaPoints = InitialCharacterStamina;
            MaxStaminaPoints = InitialCharacterStamina;

            SavePlayerStamina();
        }


        public static void SavePlayerStamina()
        {
            ES3.Save("StaminaPoints", StaminaPoints, GetSaveFilePath());
            ES3.Save("MaxStaminaPoints", MaxStaminaPoints, GetSaveFilePath());
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
    }
}
