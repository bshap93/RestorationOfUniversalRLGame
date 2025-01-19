using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Core.Events;
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

        // Clear existing UI elements to avoid duplicates
        foreach (Transform child in recipeListParent.transform) Destroy(child.gameObject);

        CookingRepiceIds.Clear(); // Clear the list to rebuild correctly

        foreach (var recipe in JournalPersistenceManager.JournalData.knownRecipes)
        {
            if (CookingRepiceIds.Contains(recipe.recipeID))
                continue;

            var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

            CookingRepiceIds.Add(recipe.recipeID);

            var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
            if (recipeEntryScript != null)
                recipeEntryScript.SetRecipe(recipe);
        }
    }


    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(RecipeEvent @event)
    {
        if (@event.EventType == RecipeEventType.RecipeLearned)
        {
            if (CookingRepiceIds.Contains(@event.RecipeParameter.recipeID))
            {
                Debug.LogWarning($"Duplicate recipe ignored: {@event.RecipeParameter.recipeName}");
                return;
            }

            var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

            CookingRepiceIds.Add(@event.RecipeParameter.recipeID);

            var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
            if (recipeEntryScript != null)
                recipeEntryScript.SetRecipe(@event.RecipeParameter);
        }
    }
}
