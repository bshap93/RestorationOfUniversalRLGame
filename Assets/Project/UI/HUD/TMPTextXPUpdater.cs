using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.Player;
using TMPro;
using UnityEngine;

namespace Project.UI.HUD
{
    public class TMPTextXpUpdater : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        [SerializeField] TMP_Text xpText; // The TMP Text that shows the level and XP
        [SerializeField] PlayerStats playerStats; // Reference to the PlayerStats

        void OnEnable()
        {
            this.MMEventStartListening();

            // Subscribe to OnStatsUpdated event from ResourcesPersistenceManager
            var manager = FindObjectOfType<ResourcesPersistenceManager>();
            if (manager != null) manager.OnStatsUpdated += RefreshXPText;
        }

        void OnDisable()
        {
            this.MMEventStopListening();

            if (playerStats != null)
            {
                playerStats.XpManager.OnExperienceChanged -= UpdateXPText;
                playerStats.XpManager.OnLevelChanged -= UpdateXPText;
            }

            // Unsubscribe from OnStatsUpdated event
            var manager = FindObjectOfType<ResourcesPersistenceManager>();
            if (manager != null) manager.OnStatsUpdated -= RefreshXPText;
        }

        /// <summary>
        ///     Called when the MMCameraEvent is received (for SetTargetCharacter)
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                Debug.Log("TMPTextXPUpdater: SetTargetCharacter event received.");

                if (playerStats != null)
                {
                    playerStats.XpManager.OnExperienceChanged -= UpdateXPText;
                    playerStats.XpManager.OnLevelChanged -= UpdateXPText;
                }

                var newCharacter = eventType.TargetCharacter.gameObject;
                if (newCharacter != null)
                {
                    playerStats = newCharacter.GetComponent<PlayerStats>();

                    if (playerStats != null && playerStats.XpManager != null)
                    {
                        playerStats.XpManager.OnExperienceChanged += UpdateXPText;
                        playerStats.XpManager.OnLevelChanged += UpdateXPText;
                        UpdateXPText(playerStats.XpManager.playerExperiencePoints);
                    }
                    else
                    {
                        Debug.LogError(
                            "TMPTextXPUpdater: PlayerStats or XPManager component not found on new target character.");
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the TMP Text to display the current level, XP, and XP required for the next level.
        /// </summary>
        /// <param name="newXP">The new XP value of the player</param>
        void UpdateXPText(int newXP)
        {
            if (playerStats == null || playerStats.XpManager == null)
            {
                Debug.LogError("TMPTextXPUpdater: PlayerStats or XPManager is null.");
                return;
            }

            var currentLevel = playerStats.XpManager.playerCurrentLevel;
            var currentXP = playerStats.XpManager.playerExperiencePoints;
            var requiredXP = playerStats.XpManager.playerXpForNextLevel;

            xpText.text = $"LVL: {currentLevel} Exp: {currentXP} / {requiredXP}";
        }

        /// <summary>
        ///     Refreshes the XP text when OnStatsUpdated is invoked.
        /// </summary>
        void RefreshXPText()
        {
            if (playerStats != null && playerStats.XpManager != null)
                UpdateXPText(playerStats.XpManager.playerExperiencePoints);
        }
    }
}
