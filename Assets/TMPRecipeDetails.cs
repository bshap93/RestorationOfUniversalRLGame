using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TMPRecipeDetails : MonoBehaviour, MMEventListener<RecipeEvent>
{
    public Image recipeImage;
    public TMP_Text recipeName;
    public TMP_Text recipeDescription;

    public List<GameObject> requiredMaterialsEntries;
    public List<GameObject> toolsEntries;
    public List<GameObject> requiredRawFoodItemsEntries;

    CookingRecipe _recipe;

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
        if (recipeEvent.EventType == RecipeEventType.ShowRecipeDetails)
        {
            _recipe = recipeEvent.RecipeParameter;
            SetRecipeDetails();
        }
    }


    void SetRecipeDetails()
    {
        recipeName.text = _recipe.recipeName;
        recipeImage.sprite = _recipe.finishedFoodItem.FinishedFood.Icon;

        ///
        /// 
    }
}
