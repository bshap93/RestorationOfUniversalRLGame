using System.Collections.Generic;
using Core.Events;
using Gameplay.ItemsInteractions;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Crafting.Cooking.Recipes
{
    public class RecipeDisplay : MonoBehaviour, MMEventListener<RecipeEvent>
    {
        [SerializeField] GameObject recipeEntryPrefab;
        [SerializeField] GameObject recipeListParent;

        [FormerlySerializedAs("journalPersistenceManager")]
        [FormerlySerializedAs("_journalPersistenceManager")]
        [SerializeField]
        CraftingRecipeManager craftingRecipeManager;
        readonly List<string> RecipeIds = new();

        void OnEnable()
        {
            this.MMEventStartListening();

            craftingRecipeManager = FindFirstObjectByType<CraftingRecipeManager>();


            // Clear existing UI elements to avoid duplicates
            foreach (Transform child in recipeListParent.transform) Destroy(child.gameObject);

            RecipeIds.Clear(); // Clear the list to rebuild correctly

            foreach (var recipe in CraftingRecipeManager.GetAllKnownRecipes())
            {
                if (RecipeIds.Contains(recipe.Item.ItemID))
                    continue;

                var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

                RecipeIds.Add(recipe.Item.ItemID);

                var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
                if (recipeEntryScript != null)
                    recipeEntryScript.SetRecipe(recipe);
            }
        }


        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(RecipeEvent mmEvent)
        {
            if (mmEvent.EventType == RecipeEventType.RecipeLearned)
            {
                if (RecipeIds.Contains(mmEvent.RecipeParameter.Item.ItemID))
                {
                    Debug.LogWarning($"Duplicate recipe ignored: {mmEvent.RecipeParameter.Item.ItemID}");
                    return;
                }

                var recipeEntry = Instantiate(recipeEntryPrefab, recipeListParent.transform);

                RecipeIds.Add(mmEvent.RecipeParameter.Item.ItemID);

                var recipeEntryScript = recipeEntry.GetComponent<RecipeEntry>();
                if (recipeEntryScript != null)
                    recipeEntryScript.SetRecipe(mmEvent.RecipeParameter);
            }
        }
    }
}
