using Gameplay.Player;
using Gameplay.SaveLoad;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
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
            this.MMEventStartListening();

            // Subscribe to OnStatsUpdated event from ResourcesPersistenceManager
            var manager = FindObjectOfType<ResourcesPersistenceManager>();
            if (manager != null) manager.OnStatsUpdated += RefreshCurrencyText;
        }

        void OnDisable()
        {
            this.MMEventStopListening();

            if (playerStats != null) playerStats.OnCurrencyChanged -= UpdateCurrencyText;

            // Unsubscribe from OnStatsUpdated event
            var manager = FindObjectOfType<ResourcesPersistenceManager>();
            if (manager != null) manager.OnStatsUpdated -= RefreshCurrencyText;
        }

        /// <summary>
        ///     Called when the MMCameraEvent is received (for SetTargetCharacter)
        /// </summary>
        /// <param name="mmEvent"></param>
        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (playerStats != null) playerStats.OnCurrencyChanged -= UpdateCurrencyText;

                var newCharacter = mmEvent.TargetCharacter.gameObject;
                if (newCharacter != null)
                {
                    playerStats = newCharacter.GetComponent<PlayerStats>();

                    if (playerStats != null)
                    {
                        playerStats.OnCurrencyChanged += UpdateCurrencyText;
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
            currencyText.text = $"{newCurrencyAmount}";
        }

        /// <summary>
        ///     Refreshes the currency text when OnStatsUpdated is invoked.
        /// </summary>
        void RefreshCurrencyText()
        {
            if (playerStats != null) UpdateCurrencyText(playerStats.playerCurrency);
        }
    }
}
