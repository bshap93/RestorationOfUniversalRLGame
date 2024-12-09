using MoreMountains.Tools;
using Project.Gameplay.Player.Health;
using UnityEngine;

namespace Project.Gameplay.Player.Stats
{
    public class XpManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public int playerExperiencePoints;
        public int playerCurrentLevel;
        public int playerXpForNextLevel;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "PlayerExperienceChanged")
            {
                var newXp = eventType.IntParameter;
                AddExperience(newXp);
            }
        }


        public void Initialize()
        {
            playerCurrentLevel = 1;
            playerExperiencePoints = 0;
            playerXpForNextLevel = 20;
        }

        /// <summary>
        ///     Adds experience to the player and triggers an MMGameEvent to notify listeners.
        /// </summary>
        /// <param name="experience">Amount of experience to add</param>
        public void AddExperience(int experience)
        {
            playerExperiencePoints += experience;
            Debug.Log($"Player gained {experience} experience points.");


            if (playerExperiencePoints >= playerXpForNextLevel) LevelUp();
        }

        /// <summary>
        ///     Handles leveling up the player and triggers an MMGameEvent to notify listeners.
        /// </summary>
        public void LevelUp()
        {
            playerCurrentLevel++;

            playerExperiencePoints -= playerXpForNextLevel;

            playerXpForNextLevel = Mathf.RoundToInt(20 * Mathf.Pow(1.5f, playerCurrentLevel - 1));


            ApplyLevelUpBonuses();
        }

        /// <summary>
        ///     Applies level-up bonuses to the player's health and other stats.
        /// </summary>
        void ApplyLevelUpBonuses()
        {
            var playerHealth = gameObject.GetComponent<HealthAlt>();
            if (playerHealth != null)
            {
                playerHealth.SetMaximumHealth(playerHealth.MaximumHealth + 10);
                playerHealth.CurrentHealth = Mathf.Round((playerHealth.CurrentHealth + playerHealth.MaximumHealth) / 2);
                Debug.Log($"Player leveled up to level {playerCurrentLevel}.");
            }
        }
    }
}
