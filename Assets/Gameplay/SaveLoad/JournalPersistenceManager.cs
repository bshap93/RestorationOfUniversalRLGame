﻿#if UNITY_EDITOR
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
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
    const string JournalFileName = "PlayerJournal.save";
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

    public void OnMMEvent(MMGameEvent itemEvent)
    {
        if (itemEvent.EventName == "SaveJournal")
            SaveJournal();
        else if (itemEvent.EventName == "RevertJournal") RevertJournalToLastSave();
    }
    public void OnMMEvent(RecipeEvent itemEvent)
    {
        if (itemEvent.EventType == RecipeEventType.RecipeLearned)
            AddRecipeToJournal(itemEvent.RecipeParameter);
    }

    public void AddRecipeToJournal(CookingRecipe recipe)
    {
        if (!JournalData.knownRecipes.Exists(r => r.recipeID == recipe.recipeID))
            JournalData.knownRecipes.Add(recipe);
        else
            Debug.LogWarning($"Duplicate recipe not added: {recipe.recipeName}");
    }


    public void SaveJournal()
    {
        ES3.Save("JournalData", JournalData, "PlayerJournal.save");
    }

    public void RevertJournalToLastSave()
    {
        if (ES3.FileExists("PlayerJournal.save"))
            JournalData = ES3.Load<JournalData>("JournalData", "PlayerJournal.save");
        else
            Debug.LogWarning("Save file not found.");
    }

    public static void ResetJournal()
    {
        ES3.DeleteFile("PlayerJournal.save");


        Debug.Log("Journal reset.");
    }


    public JournalData GetJournalData()
    {
        return JournalData;
    }
    public bool HasSavedData()
    {
        // Check for journal-specific save files
        return ES3.FileExists("PlayerJournal.save");
    }
}
