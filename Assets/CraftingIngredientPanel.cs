using System.Collections.Generic;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using UnityEngine;
using UnityEngine.Serialization;

public class CraftingIngredientPanel : MonoBehaviour
{
    [FormerlySerializedAs("_itemIngredientElements")]
    public ItemIngredientElement[] itemIngredientElements;
    List<Ingredient> _ingredients;

    void Start()
    {
        itemIngredientElements = GetComponentsInChildren<ItemIngredientElement>();

        for (var i = 0; i < itemIngredientElements.Length; i++) itemIngredientElements[i].Nullify();

        if (_ingredients != null) Initialize(_ingredients);
    }

    public void Initialize(List<Ingredient> ingredientsLoc)
    {
        _ingredients = ingredientsLoc;
        for (var i = 0; i < itemIngredientElements.Length; i++)
            if (i < ingredientsLoc.Count)
                itemIngredientElements[i].Initialize(ingredientsLoc[i].Item.Icon, ingredientsLoc[i].Quantity);
            else
                itemIngredientElements[i].Nullify();
    }

    public void Nullify()
    {
        for (var i = 0; i < itemIngredientElements.Length; i++) itemIngredientElements[i].Nullify();
    }
}
