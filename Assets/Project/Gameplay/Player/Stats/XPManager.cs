using System;
using Project.Gameplay.Player.Health;
using UnityEngine;

namespace Project.Gameplay.Player.Stats
{
    public class XpManager : MonoBehaviour
    {
        public int playerExperiencePoints;
        public int playerCurrentLevel;
        public int playerXpForNextLevel;

        public event Action<int> OnLevelChanged;
        public event Action<int> OnExperienceChanged;

        public void Initialize()
        {
            playerCurrentLevel = 1;
            playerExperiencePoints = 0;
            playerXpForNextLevel = 20;
        }

        public void AddExperience(int experience)
        {
            playerExperiencePoints += experience;
            Debug.Log($"Player gained {experience} experience points.");

            OnExperienceChanged?.Invoke(playerExperiencePoints);

            if (playerExperiencePoints >= playerXpForNextLevel) LevelUp();
        }

        public void LevelUp()
        {
            playerCurrentLevel++;

            playerExperiencePoints -= playerXpForNextLevel;

            playerXpForNextLevel = (playerCurrentLevel + 1) * 20;

            OnLevelChanged?.Invoke(playerCurrentLevel);

            ApplyLevelUpBonuses();
        }

        void ApplyLevelUpBonuses()
        {
            // Increase max health by 10
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
