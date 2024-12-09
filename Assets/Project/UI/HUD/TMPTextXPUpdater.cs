using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
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
            // Listen for the SetTargetCharacter event
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Unsubscribe from all events
            this.MMEventStopListening();

            if (playerStats != null)
            {
                playerStats.XpManager.OnExperienceChanged -= UpdateXPText;
                playerStats.XpManager.OnLevelChanged -= UpdateXPText;
            }
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

                // Unsubscribe from the previous playerStats event, if any
                if (playerStats != null)
                {
                    playerStats.XpManager.OnExperienceChanged -= UpdateXPText;
                    playerStats.XpManager.OnLevelChanged -= UpdateXPText;
                }

                // Get PlayerStats from the new character
                var newCharacter = eventType.TargetCharacter.gameObject;
                if (newCharacter != null)
                {
                    playerStats = newCharacter.GetComponent<PlayerStats>();

                    if (playerStats != null && playerStats.XpManager != null)
                    {
                        // Subscribe to XP and Level change events
                        playerStats.XpManager.OnExperienceChanged += UpdateXPText;
                        playerStats.XpManager.OnLevelChanged += UpdateXPText;

                        // Initialize the text with the current level, current XP, and XP to next level
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
    }
}
