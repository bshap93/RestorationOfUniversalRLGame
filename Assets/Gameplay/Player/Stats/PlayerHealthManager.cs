#if UNITY_EDITOR

using Core.Events;
using Gameplay.Character;
using Gameplay.Character.Health;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Player.Stats
{
    public static class PlayerHealthManagerDebug
    {
        [MenuItem("Debug/Reset Health")]
        public static void ResetHealth()
        {
            PlayerHealthManager.ResetPlayerHealth();
        }
    }
#endif

    public class PlayerHealthManager : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        public static float HealthPoints;
        public static float MaxHealthPoints;


        public static float InitialCharacterHealth;
        public HealthBarUpdater healthBarUpdater;
        public CharacterStatProfile characterStatProfile;

        public bool ImmuneToDamage;

        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);

            if (characterStatProfile != null)
                InitialCharacterHealth = characterStatProfile.InitialMaxHealth;
            else
                Debug.LogError("CharacterStatProfile not set in PlayerHealthManager");
        }

        void Start()
        {
            _savePath = GetSaveFilePath();

            LoadPlayerHealth();
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(MMCameraEvent eventType)
        {
            // MMCameraEventTypes.SetTargetCharacter is an acuurate measure of just after the player has been spawned
            // and that then are its components initialized
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
                if (ImmuneToDamage)
                    PlayerStatusEvent.Trigger(PlayerStatusEventType.ImmuneToDamage);
        }

        public void Initialize()
        {
            Debug.Log("Initializing player health");
            ResetPlayerHealth();
            healthBarUpdater.Initialize();
        }

        public static void ConsumeHealth(float healthToConsume)
        {
            if (HealthPoints - healthToConsume < 0)
            {
                HealthPoints = 0;
                PlayerStatusEvent.Trigger(PlayerStatusEventType.OutOfHealth);
                HealthEvent.Trigger(HealthEventType.ConsumeHealth, healthToConsume);
                DialogueManager.ShowAlert("Health Depleted");
            }
            else
            {
                HealthPoints -= healthToConsume;
                HealthEvent.Trigger(HealthEventType.ConsumeHealth, healthToConsume);
            }

            SavePlayerHealth();
        }

        public static void RecoverHealth(float amount)
        {
            if (HealthPoints == 0 && amount > 0) PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedHealth);
            HealthPoints += amount;
            HealthEvent.Trigger(HealthEventType.RecoverHealth, amount);
            SavePlayerHealth();
        }

        public static void FullyRecoverHealth()
        {
            HealthPoints = MaxHealthPoints;
            PlayerStatusEvent.Trigger(PlayerStatusEventType.RegainedHealth);
            HealthEvent.Trigger(HealthEventType.RecoverHealth, MaxHealthPoints - HealthPoints);
            SavePlayerHealth();
        }

        public static void IncreaseMaximumHealth(float amount)
        {
            MaxHealthPoints += amount;
            HealthEvent.Trigger(HealthEventType.IncreaseMaximumHealth, amount);
            SavePlayerHealth();
        }

        public static void DecreaseMaximumHealth(float amount)
        {
            MaxHealthPoints -= amount;
            HealthEvent.Trigger(HealthEventType.DecreaseMaximumHealth, amount);
            SavePlayerHealth();
        }


        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "PlayerHealth.es3" : $"{slotPath}/PlayerHealth.es3";
        }

        public void LoadPlayerHealth()
        {
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(saveFilePath);
            if (exists)
            {
                HealthPoints = ES3.Load<float>("HealthPoints", saveFilePath);
                MaxHealthPoints = ES3.Load<float>("MaxHealthPoints", saveFilePath);
                healthBarUpdater.Initialize();
            }
            else
            {
                Debug.LogError("No saved health data found");
                ResetPlayerHealth();
                healthBarUpdater.Initialize();
            }
        }
        public static void ResetPlayerHealth()
        {
            HealthPoints = InitialCharacterHealth;
            MaxHealthPoints = InitialCharacterHealth;

            SavePlayerHealth();
        }

        public static void SavePlayerHealth()
        {
            ES3.Save("HealthPoints", HealthPoints, "PlayerHealth.es3");
            ES3.Save("MaxHealthPoints", MaxHealthPoints, "PlayerHealth.es3");
            Debug.Log("Player health saved: " + HealthPoints + " / " + MaxHealthPoints);
        }

        public bool HasSavedData()
        {
            return ES3.FileExists(GetSaveFilePath());
        }

        public static bool IsPlayerOutOfHealth()
        {
            return HealthPoints <= 0;
        }

        [Button(ButtonSizes.Medium)]
        public void HurtPlayer(float damage = 10)
        {
            if (ImmuneToDamage) return;
            ConsumeHealth(damage);
        }
    }
}
