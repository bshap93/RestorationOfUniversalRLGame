using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;

public class JournalDisplay : MonoBehaviour, MMEventListener<RecipeEvent>
{
    [SerializeField] GameObject recipeEntryPrefab;
    [SerializeField] GameObject recipeListParent;

    void OnEnable()
    {
        this.MMEventStartListening();
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
