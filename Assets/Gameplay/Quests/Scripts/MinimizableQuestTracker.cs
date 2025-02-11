using UnityEngine;
using UnityEngine.UI;

namespace PixelCrushers.DialogueSystem
{
    /// <summary>
    ///     Extends the StandardUIQuestTracker to add minimize/maximize functionality
    ///     while preserving all original functionality.
    /// </summary>
    public class MinimizableQuestTracker : StandardUIQuestTracker
    {
        [Header("Minimize/Maximize Settings")] [Tooltip("Key to save minimize state in PlayerPrefs")]
        public string playerPrefsMinimizeKey = "QuestTrackerMinimized";

        [Tooltip("Container that holds just the quest entries (separate from header)")]
        public Transform questEntriesContainer;

        [Tooltip("Optional UI Image that will change sprite based on minimize state")]
        public Image toggleIcon;

        [Tooltip("Sprite to show when maximized (e.g., down arrow)")]
        public Sprite maximizedSprite;

        [Tooltip("Sprite to show when minimized (e.g., up arrow)")]
        public Sprite minimizedSprite;

        protected bool isMinimized;

        public override void Start()
        {
            base.Start();

            // Initialize minimize state from PlayerPrefs
            isMinimized = !string.IsNullOrEmpty(playerPrefsMinimizeKey) &&
                          PlayerPrefs.GetInt(playerPrefsMinimizeKey, 0) == 1;

            // Set initial state
            UpdateMinimizeState();
        }

        /// <summary>
        ///     Toggles between minimized and maximized states
        /// </summary>
        public virtual void ToggleMinimize()
        {
            isMinimized = !isMinimized;

            // Save state if we have a PlayerPrefs key
            if (!string.IsNullOrEmpty(playerPrefsMinimizeKey))
                PlayerPrefs.SetInt(playerPrefsMinimizeKey, isMinimized ? 1 : 0);

            UpdateMinimizeState();
        }

        /// <summary>
        ///     Updates UI elements based on current minimize state
        /// </summary>
        protected virtual void UpdateMinimizeState()
        {
            if (questEntriesContainer != null) questEntriesContainer.gameObject.SetActive(!isMinimized);

            if (toggleIcon != null && maximizedSprite != null && minimizedSprite != null)
                toggleIcon.sprite = isMinimized ? minimizedSprite : maximizedSprite;
        }

        public override void ShowTracker()
        {
            base.ShowTracker();
            UpdateMinimizeState(); // Ensure minimize state is preserved when showing
        }

        public override void UpdateTracker()
        {
            base.UpdateTracker();
            UpdateMinimizeState(); // Ensure minimize state is preserved after updates
        }
    }
}
