using System.Collections;
using System.Collections.Generic;
using Gameplay.ItemManagement.InventoryTypes.Cooking;
using MoreMountains.Tools;
using Prefabs.UI.Displayers;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using TMPro;
using UnityEngine;

namespace Gameplay.ItemManagement.Cooking
{
    public class RecipeDisplayer : MonoBehaviour, MMEventListener<RecipeEvent>
    {
        [Tooltip("The prefab to use to display learned recipes")]
        public RecipeDisplayItem RecipeDisplayPrefab;

        [Tooltip("The duration the recipe display will remain on screen")]
        public float RecipeDisplayDuration = 5f;

        [Tooltip("The fade in/out duration")] public float FadeDuration = 0.2f;
        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }
        public void OnMMEvent(RecipeEvent mmEvent)
        {
            if (mmEvent.EventType == RecipeEventType.FinishedCookingRecipe)
                DisplayFinishedRecipe(mmEvent.RecipeParameter);
        }

        public void DisplayLearnedRecipes(List<CookingRecipe> recipes, bool areNew = true)
        {
            foreach (var recipe in recipes)
            {
                // Instantiate the display item
                var display = Instantiate(RecipeDisplayPrefab, transform);

                // Update the display text and icon
                display.DisplayLearned(recipe);

                // Optionally customize for already-known recipes
                if (!areNew) display.GetComponentInChildren<TMP_Text>().text = $"Already Known: {recipe.recipeName}";

                // Fade out and destroy the display
                StartCoroutine(FadeOutAndDestroy(display.gameObject));
            }
        }

        public void DisplayFinishedRecipe(CookingRecipe recipe)
        {
            // Instantiate the display item
            var display = Instantiate(RecipeDisplayPrefab, transform);

            // Update the display text and icon
            display.DisplayFinishedCooking(recipe);

            StartCoroutine(FadeOutAndDestroy(display.gameObject));
        }


        IEnumerator FadeOutAndDestroy(GameObject display)
        {
            Debug.Log("Starting fade out");
            var canvasGroup = display.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                yield return new WaitForSeconds(RecipeDisplayDuration);
                yield return MMFade.FadeCanvasGroup(canvasGroup, FadeDuration, 0);
            }

            Debug.Log("Destroying display");
            Destroy(display);
        }
    }
}
