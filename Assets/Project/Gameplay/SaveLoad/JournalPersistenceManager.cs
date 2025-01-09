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

        [FormerlySerializedAs("_journalData")] public JournalData journalData = new();


        void OnEnable()
        {
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<RecipeEvent>();
        }

        void OnDisable()
        {
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<RecipeEvent>();
        }

        public void OnMMEvent(MMGameEvent cookingStationEvent)
        {
            if (cookingStationEvent.EventName == "SaveJournal")
                SaveJournal();
            else if (cookingStationEvent.EventName == "RevertJournal") RevertJournalToLastSave();
        }
        public void OnMMEvent(RecipeEvent cookingStationEvent)
        {
            if (cookingStationEvent.EventType == RecipeEventType.RecipeLearned)
                AddRecipeToJournal(cookingStationEvent.RecipeParameter);
        }

        public void AddRecipeToJournal(CookingRecipe recipe)
        {
            Debug.Log("Adding recipe to journal: " + recipe.name);
            journalData.knownRecipes.Add(recipe);
        }

        public void SaveJournal()
        {
            ES3.Save("JournalData", journalData, "PlayerJournal.save");
            Debug.Log("Journal saved.");
        }

        public void RevertJournalToLastSave()
        {
            if (ES3.FileExists("PlayerJournal.save"))
            {
                journalData = ES3.Load<JournalData>("JournalData", "PlayerJournal.save");
                Debug.Log("Journal reverted to last save.");
            }
            else
            {
                Debug.LogWarning("Save file not found.");
            }
        }


        public JournalData GetJournalData()
        {
            return journalData;
        }
    }
}
