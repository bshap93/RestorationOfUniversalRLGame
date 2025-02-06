#if UNITY_EDITOR
using Gameplay.ItemManagement.InventoryTypes.Cooking;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.SaveLoad.Journal;
using UnityEditor;
using UnityEngine;

public static class JournalRecipeDebug
{
    [MenuItem("Debug/Reset Journal Recipes")]
    public static void ResetJournalRecipesMenu()
    {
        JournalPersistenceManager.ResetJournal();
    }
}
#endif


public class JournalPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>, MMEventListener<RecipeEvent>
{
    const string SaveFolderName = "Player";

    public JournalData JournalData;

    void Awake()
    {
        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);
    }


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

    public void OnMMEvent(MMGameEvent mmEvent)
    {
        if (mmEvent.EventName == "SaveJournal")
            SaveJournal();
        else if (mmEvent.EventName == "RevertJournal") RevertJournalToLastSave();
    }
    public void OnMMEvent(RecipeEvent mmEvent)
    {
        if (mmEvent.EventType == RecipeEventType.RecipeLearned)
            AddRecipeToJournal(mmEvent.RecipeParameter);
    }

    static string GetSaveFilePathJournal()
    {
        var slotPath = ES3SlotManager.selectedSlotPath;
        return string.IsNullOrEmpty(slotPath) ? "PlayerJournal.es3" : $"{slotPath}/PlayerJournal.es3";
    }

    public void AddRecipeToJournal(CookingRecipe recipe)
    {
        if (!JournalData.knownRecipes.Exists(r => r.recipeID == recipe.recipeID))
        {
            JournalData.knownRecipes.Add(recipe);
            Debug.Log($"Recipe {recipe.recipeName} added to the journal.");
        }
        else
        {
            Debug.Log($"Recipe {recipe.recipeName} is already in the journal.");
        }
    }


    public void SaveJournal()
    {
        ES3.Save("JournalData", JournalData, GetSaveFilePathJournal());
    }

    public void RevertJournalToLastSave()
    {
        if (ES3.FileExists(GetSaveFilePathJournal()))
            JournalData = ES3.Load<JournalData>("JournalData", GetSaveFilePathJournal());
        else
            Debug.LogWarning("Save file not found.");
    }

    public static void ResetJournal()
    {
        ES3.DeleteFile(GetSaveFilePathJournal());


        Debug.Log("Journal reset.");
    }


    public JournalData GetJournalData()
    {
        return JournalData;
    }
    public bool HasSavedData()
    {
        // Check for journal-specific save files
        return ES3.FileExists(GetSaveFilePathJournal());
    }
}
