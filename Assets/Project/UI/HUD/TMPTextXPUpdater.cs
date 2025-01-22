using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Player;
using TMPro;
using UnityEngine;

namespace Project.UI.HUD
{
    public class TMPTextXpUpdater : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListener<MMCameraEvent>
    {
        [SerializeField] TMP_Text xpText; // The TMP Text that shows the level and XP

        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<MMCameraEvent>();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<MMCameraEvent>();
        }

        /// <summary>
        ///     Called when the MMCameraEvent is received (for SetTargetCharacter)
        /// </summary>
        /// <param name="mmEvent">The event type</param>
        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                var newCharacter = mmEvent.TargetCharacter.gameObject;
                if (newCharacter != null)
                {
                    var playerStats = newCharacter.GetComponent<PlayerStats>();
                    if (playerStats != null && playerStats.XpManager != null)
                        UpdateXPText(playerStats.XpManager.playerExperiencePoints);
                    else
                        Debug.LogError(
                            "TMPTextXPUpdater: PlayerStats or XPManager component not found on new target character.");
                }
            }
        }

        /// <summary>
        ///     Handles MMGameEvents for PlayerExperienceChanged and PlayerLevelChanged
        /// </summary>
        /// <param name="mmEvent">The event type</param>
        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "PlayerExperienceChanged")
                UpdateXPText(mmEvent.IntParameter); // Update using new XP
        }

        /// <summary>
        ///     Updates the TMP Text to display the current level, XP, and XP required for the next level.
        /// </summary>
        /// <param name="newXP">The new XP value of the player, or -1 if only updating level</param>
        void UpdateXPText(int newXP)
        {
            // Use the MMGameEvent system for updates, so no direct reference to PlayerStats is required
            var currentLevel = 1; // Default value if we don't get an updated level
            var currentXP = 0; // Default value for current XP
            var requiredXP = 0; // Default value for required XP

            // Attempt to find PlayerStats if possible (useful for character switches)
            var playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null && playerStats.XpManager != null)
            {
                currentLevel = playerStats.XpManager.playerCurrentLevel;
                currentXP = playerStats.XpManager.playerExperiencePoints;
                requiredXP = playerStats.XpManager.playerXpForNextLevel;
            }

            // Update the XP text
            xpText.text = $"LVL: {currentLevel} Exp: {currentXP} / {requiredXP}";
        }
    }
}
