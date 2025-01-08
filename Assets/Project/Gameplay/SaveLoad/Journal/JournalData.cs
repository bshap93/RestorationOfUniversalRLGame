using System;
using System.Collections.Generic;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;

namespace Project.Gameplay.SaveLoad.Journal
{
    [Serializable]
    public class JournalData
    {
        public List<CookingRecipe> knownRecipes = new();
    }
}
