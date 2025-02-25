#if UNITY_EDITOR

using Core.Events;
using Gameplay.Character;
using Gameplay.Character.Stamina;
using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Player.Stats
{
    public static class PlayerStaminaManagerDebug
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
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static float InitialCharacterStamina;
        public StaminaBarUpdater staminaBarUpdater;
        public CharacterStatProfile characterStatProfile;


        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (characterStatProfile != null)
                InitialCharacterStamina = characterStatProfile.InitialMaxStamina;
            else
                Debug.LogError("CharacterStatProfile not set in PlayerStaminaManager");
        }

        void Start()
        {
            _savePath = GetSaveFilePath();

            LoadPlayerStamina();
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
            ResetPlayerStamina();
            staminaBarUpdater.Initialize();
        }

        public static void ConsumeStamina(float amount)
        {
            if (StaminaPoints - amount < 0)
            {
                StaminaPoints = 0;
                PlayerStatusEvent.Trigger(PlayerStatusEventType.OutOfStamina);
                DialogueManager.ShowAlert("Stamina Depleted");
            }
            else
            {
                StaminaPoints -= amount;
            }

            SavePlayerStamina();
        }

        public static void RecoverStamina(float amount)
        {
            if (StaminaPoints == 0 && amount > 0) PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedStamina);
            StaminaPoints += amount;
            SavePlayerStamina();
        }

        public static void FullyRecoverStamina()
        {
            StaminaPoints = MaxStaminaPoints;
            PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedStamina);

            SavePlayerStamina();
        }

        public static void IncreaseMaximumStamina(float amount)
        {
            MaxStaminaPoints += amount;
            SavePlayerStamina();
        }

        public static void DecreaseMaximumStamina(float amount)
        {
            MaxStaminaPoints -= amount;
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
                staminaBarUpdater.Initialize();
            }
            else
            {
                Debug.LogError("No saved stamina data found");
                ResetPlayerStamina();
                staminaBarUpdater.Initialize();
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
            Debug.Log("Saved player stamina: " + StaminaPoints + " / " + MaxStaminaPoints);
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }
        public static bool IsPlayerOutOfStamina()
        {
            return StaminaPoints <= 0;
        }
    }
}
