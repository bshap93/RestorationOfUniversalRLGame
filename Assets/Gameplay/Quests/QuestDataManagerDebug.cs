#if UNITY_EDITOR
using PixelCrushers;
using UnityEditor;
using UnityEngine;

namespace Project.Gameplay.Quests
{
    public static class QuestDataManagerDebug
    {
        [MenuItem("Debug/Reset Quest Data")]
        public static void ResetQuestDataMenu()
        {
            // Call your save system's reset method for quest data
            ResetQuestData();

            Debug.Log("[QuestDataManager] Quest data has been reset.");
        }

        static void ResetQuestData()
        {
            // Example: Replace this with the actual implementation to reset quest data
            SaveSystem.ResetGameState(); // Adjust this to your exact method for quest reset
        }
    }
}
#endif
