using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Prefabs.UI.Displayers;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

namespace Gameplay.ItemManagement.Cooking
{
    public class RecipeDisplayer : MonoBehaviour
    {
        [Tooltip("The prefab to use to display learned recipes")]
        public RecipeDisplayItem RecipeDisplayPrefab;

        [Tooltip("The duration the recipe display will remain on screen")]
        public float RecipeDisplayDuration = 5f;

        [Tooltip("The fade in/out duration")] public float FadeDuration = 0.2f;

        public void DisplayLearnedRecipes(List<CookingRecipe> recipes)
        {
            foreach (var recipe in recipes)
            {
                // Instantiate and show the recipe
                var display = Instantiate(RecipeDisplayPrefab, transform);
                display.Display(recipe);

                // Fade out and destroy the display
                StartCoroutine(FadeOutAndDestroy(display.gameObject));
            }
        }

        IEnumerator FadeOutAndDestroy(GameObject display)
        {
            var canvasGroup = display.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                yield return new WaitForSeconds(RecipeDisplayDuration);
                yield return MMFade.FadeCanvasGroup(canvasGroup, FadeDuration, 0);
            }

            Destroy(display);
        }
    }
}
