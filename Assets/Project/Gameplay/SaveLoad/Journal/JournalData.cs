using System;
using System.Collections.Generic;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;

namespace Project.Gameplay.SaveLoad.Journal
{
    [Serializable]
    public class JournalData
    {
        public List<SerializableRecipe> knownRecipes = new();
    }

    [Serializable]
    public class SerializableRecipe
    {
        public string recipeName;
        public string recipeID; // or any unique identifier

        public SerializableRecipe(CookingRecipe recipe)
        {
            recipeName = recipe.recipeName; // Adjust for your CookingRecipe implementation
            recipeID = recipe.recipeID; // Adjust for your CookingRecipe implementation
        }
    }
}
