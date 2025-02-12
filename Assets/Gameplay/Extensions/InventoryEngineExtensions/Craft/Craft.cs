using System;
using System.Linq;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.Craft
{
    [Serializable]
    public class Ingredient : ISerializationCallbackReceiver
    {
        [HideInInspector] public string Name;
        public InventoryItem Item;
        public int Quantity;


        public void OnBeforeSerialize()
        {
            Name = ToString();
        }
        public void OnAfterDeserialize()
        {
            Name = ToString();
        }
        public override string ToString()
        {
            return (Quantity == 1 ? "" : Quantity + " ") + (Item == null ? "null" : Item.ItemName) +
                   (Quantity > 1 ? "s" : "");
        }
    }

    [Serializable]
    public class Recipe : Ingredient
    {
        public Ingredient[] Ingredients;
        public string IngredientsText => string.Join(", ", Ingredients.Select(ingredient => ingredient.Name));
    }

    public static class Crafting
    {
        public static bool ContainsIngredientsForRecipe(this Inventory inventory, Recipe recipe)
        {
            if (inventory == null) return false;
            return !recipe.Ingredients.Any(
                ingredient =>
                    inventory.InventoryContains(ingredient.Item.ItemID)
                        .Sum(index => inventory.Content[index].Quantity) < ingredient.Quantity);
        }

        public static void Craft(this Inventory inventory, Recipe recipe)
        {
            if (inventory == null)
            {
                Debug.LogError("Cannot craft: inventory is null");
                return;
            }

            if (!inventory.ContainsIngredientsForRecipe(recipe))
            {
                Debug.Log("Missing required ingredients for recipe");
                return;
            }

            try
            {
                // Remove ingredients first
                foreach (var ingredient in recipe.Ingredients)
                    if (!inventory.RemoveItemByID(ingredient.Item.ItemID, ingredient.Quantity))
                    {
                        Debug.LogError($"Failed to remove ingredient: {ingredient.Item.ItemID}");
                        // Try to restore removed ingredients
                        foreach (var ing in recipe.Ingredients)
                            inventory.AddItem(ing.Item, ing.Quantity);

                        return;
                    }

                // Add the crafted item
                if (inventory.AddItem(recipe.Item, recipe.Quantity))
                {
                    MMInventoryEvent.Trigger(
                        MMInventoryEventType.Pick, null, string.Empty, recipe.Item, recipe.Quantity, 0, "Player1");

                    Debug.Log($"Successfully crafted: {recipe.Item.ItemName}");
                }
                else
                {
                    Debug.LogError("Failed to add crafted item");
                    // Restore ingredients if we couldn't add the result
                    foreach (var ingredient in recipe.Ingredients)
                        inventory.AddItem(ingredient.Item, ingredient.Quantity);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error during crafting: {e.Message}");
            }
        }
    }

    [CreateAssetMenu]
    public class Craft : ScriptableObject
    {
        public Recipe[] Recipes;
    }
}
