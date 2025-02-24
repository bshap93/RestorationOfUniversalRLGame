using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;

namespace Gameplay.UI.HUD
{
    public class TMPTextXpUpdater : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        [SerializeField] TMP_Text xpText; // The TMP Text that shows the level and XP

        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening();
        }

        /// <summary>
        ///     Called when the MMCameraEvent is received (for SetTargetCharacter)
        /// </summary>
        /// <param name="mmEvent">The event type</param>
        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                // var newCharacter = mmEvent.TargetCharacter.gameObject;
                // if (newCharacter != null)
                // {
                //     var playerStats = newCharacter.GetComponent<PlayerStats>();
                //     if (playerStats != null && playerStats.XpManager != null)
                //         UpdateXPText(playerStats.XpManager.playerExperiencePoints);
                //     else
                //         Debug.LogError(
                //             "TMPTextXPUpdater: PlayerStats or XPManager component not found on new target character.");
                // }
            }
        }
    }
}
