using Project.Core.Events;
using Project.Gameplay.SaveLoad;
using UnityEngine;

public class JournalDisplay : MonoBehaviour
{
    [SerializeField] GameObject recipeEntryPrefab;
    [SerializeField] GameObject recipeListParent;

    [SerializeField] JournalPersistenceManager journalPersistenceManager;

    void OnEnable()
    {
        Debug.Log("JournalDisplay.OnEnable");
        foreach (var recipe in journalPersistenceManager.journalData.knownRecipes)
        {
            // Instantiate the prefab
            var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

            // Get the script responsible for updating the UI
            var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
            if (recipeEntryScript != null)
                // Pass the recipe data directly to the UI
                recipeEntryScript.SetRecipe(recipe);
        }
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
