using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Player;
using TMPro;
using UnityEngine;

namespace Project.UI.HUD
{
    public class TMPTextCurrencyUpdater : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        [SerializeField] TMP_Text currencyText; // The TMP Text that shows currency
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

            if (playerStats != null) playerStats.OnCurrencyChanged -= UpdateCurrencyText;
        }

        /// <summary>
        ///     Called when the MMCameraEvent is received (for SetTargetCharacter)
        /// </summary>
        /// <param name="eventType"></param>
        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                Debug.Log("TMPTextCurrencyUpdater: SetTargetCharacter event received.");

                // Unsubscribe from the previous playerStats event, if any
                if (playerStats != null) playerStats.OnCurrencyChanged -= UpdateCurrencyText;

                // Get PlayerStats from the new character
                var newCharacter = eventType.TargetCharacter.gameObject;
                if (newCharacter != null)
                {
                    playerStats = newCharacter.GetComponent<PlayerStats>();

                    if (playerStats != null)
                    {
                        // Subscribe to currency change event
                        playerStats.OnCurrencyChanged += UpdateCurrencyText;

                        // Initialize the text with the current currency
                        UpdateCurrencyText(playerStats.playerCurrency);
                    }
                    else
                    {
                        Debug.LogError(
                            "TMPTextCurrencyUpdater: PlayerStats component not found on new target character.");
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the TMP Text to display the current currency amount.
        /// </summary>
        /// <param name="newCurrencyAmount">The new value of the player's currency</param>
        void UpdateCurrencyText(int newCurrencyAmount)
        {
            currencyText.text = $"Coins: {newCurrencyAmount}";
        }
    }
}
