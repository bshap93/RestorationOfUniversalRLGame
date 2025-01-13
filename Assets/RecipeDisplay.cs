using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.SaveLoad;
using UnityEngine;

public class RecipeDisplay : MonoBehaviour, MMEventListener<RecipeEvent>
{
    [SerializeField] GameObject recipeEntryPrefab;
    [SerializeField] GameObject recipeListParent;

    [SerializeField] JournalPersistenceManager journalPersistenceManager;
    readonly List<string> CookingRepiceIds = new();

    void OnEnable()
    {
        this.MMEventStartListening();
        Debug.Log("JournalDisplay.OnEnable");
        foreach (var recipe in journalPersistenceManager.journalData.knownRecipes)
        {
            // Instantiate the prefab only if it's not already in the list
            if (CookingRepiceIds.Contains(recipe.recipeID))
                continue;


            var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

            CookingRepiceIds.Add(recipe.recipeID);

            // Get the script responsible for updating the UI
            var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
            if (recipeEntryScript != null)
                // Pass the recipe data directly to the UI
                recipeEntryScript.SetRecipe(recipe);
        }
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }


    public void OnMMEvent(RecipeEvent recipeEvent)
    {
        if (recipeEvent.EventType == RecipeEventType.RecipeLearned)
        {
            // Instantiate the prefab
            var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

            // Get the script responsible for updating the UI (assume it's named RecipeEntry)
            var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
            if (recipeEntryScript != null)
                // Pass the recipe data to the UI
                recipeEntryScript.SetRecipe(recipeEvent.RecipeParameter);
        }
    }
}
