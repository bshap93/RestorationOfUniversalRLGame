using PixelCrushers;
using PixelCrushers.QuestMachine;
using UnityEditor;
using UnityEngine;

namespace Project.Gameplay.Quests
{
    public class QuestResetManager : MonoBehaviour
    {
        [MenuItem("Debug/Reset All Quest Progress")]
        public static void ResetAllQuestProgress()
        {
            // Clear all quest journals
            var journals = FindObjectsOfType<QuestJournal>();
            foreach (var journal in journals)
            {
                if (journal.questList != null)
                    journal.questList.Clear();

                if (journal.questList != null) journal.questList.Clear(); // If a specific method is available
            }

            // Reset all quest givers to their initial state
            var questGivers = FindObjectsOfType<QuestGiver>();
            foreach (var questGiver in questGivers)
                questGiver.ResetToOriginalState();

            // Unregister all quest instances
            QuestMachine.UnregisterAllQuestInstances();

            // Clear save system quest state
            SaveSystem.ResetGameState();

            Debug.Log("All quest progress has been reset.");
        }
    }
}
