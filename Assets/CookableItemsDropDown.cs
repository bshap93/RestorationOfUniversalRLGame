using Michsky.MUIP;
using MoreMountains.Tools;
using Project.Core.Events;
using UnityEngine;

public class CookableItemsDropDown : MonoBehaviour, MMEventListener<RecipeEvent>
{
    CustomDropdown _dropdown;

    void Start()
    {
        _dropdown = GetComponent<CustomDropdown>();
    }
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
        if (recipeEvent.EventType == RecipeEventType.RecipeCookableWithCurrentIngredients)
        {
            _dropdown.CreateNewItem(
                recipeEvent.RecipeParameter.recipeName,
                recipeEvent.RecipeParameter.finishedFoodItem.FinishedFood.Icon, true);

            _dropdown.SetupDropdown();
        }
    }
}
