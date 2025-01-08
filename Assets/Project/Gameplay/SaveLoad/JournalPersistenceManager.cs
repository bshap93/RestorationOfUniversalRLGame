using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.SaveLoad.Journal;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.SaveLoad
{
    public class JournalPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListener<RecipeEvent>
    {
        const string JournalFileName = "PlayerJournal.save";
        const string SaveFolderName = "Player";

        [FormerlySerializedAs("_journalData")] [SerializeField]
        JournalData journalData = new();

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent recipeEvent)
        {
            if (recipeEvent.EventName == "SaveJournal")
                SaveJournal();
            else if (recipeEvent.EventName == "RevertJournal") RevertJournalToLastSave();
        }
        public void OnMMEvent(RecipeEvent recipeEvent)
        {
            if (recipeEvent.EventType == RecipeEventType.RecipeLearned) 
                AddRecipeToJournal(recipeEvent.RecipeParameter);
        }

        public void AddRecipeToJournal(CookingRecipe recipe)
        {
            journalData.knownRecipes.Add(recipe);
        }

        public void SaveJournal()
        {
            MMSaveLoadManager.Save(journalData, JournalFileName, SaveFolderName);
            Debug.Log("Journal saved.");
        }

        public void RevertJournalToLastSave()
        {
            var loadedData =
                MMSaveLoadManager.Load(typeof(JournalData), JournalFileName, SaveFolderName) as JournalData;

            if (loadedData != null)
            {
                journalData = loadedData;
                Debug.Log("Journal reverted to last save.");
            }
        }

        public JournalData GetJournalData()
        {
            return journalData;
        }
    }
}
