#if UNITY_EDITOR
using System.Collections.Generic;
using Gameplay.Extensions.InventoryEngineExtensions.Craft;
using UnityEditor;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public static class CraftingRecipeManagerDebug
    {
        [MenuItem("Debug/Reset Picked Items")]
        public static void ResetPickedItemsMenu()
        {
            PickableManager.ResetPickedItems();
        }
    }
#endif
    public class CraftingRecipeManager : MonoBehaviour
    {
        public static HashSet<string> LearnedCraftGroups = new();
        public string[] InitialCraftGroups;

        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _savePath = GetSaveFilePath();
            foreach (var craftGroup in InitialCraftGroups)
            {
                LearnedCraftGroups.Add(craftGroup);
                SaveLearnedCraftGroup(craftGroup, true);
                Debug.Log("CraftingRecipeManager: Added initial craft group: " + craftGroup);
            }

            LoadLearnedCraftingGroups();
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "LearnedCraftGroups.es3" : $"{slotPath}/LearnedCraftGroups.es3";
        }

        public void LoadLearnedCraftingGroups()
        {
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(_savePath);
            if (exists)
            {
                var keys = ES3.GetKeys(_savePath);
                foreach (var key in keys)
                    if (ES3.Load<bool>(key, _savePath))
                        LearnedCraftGroups.Add(key);
            }
        }

        public static void ResetLearnedCraftingGroups()
        {
            var saveFilePath = GetSaveFilePath();
            // Delete the Easy Save file storing picked items
            ES3.DeleteFile(GetSaveFilePath());

            LearnedCraftGroups.Clear();
        }

        // Convert List of Recipes to RecipeGroup
        public static RecipeGroup ConvertToRecipeGroup(Recipe[] recipes, string uniqueID)
        {
            var recipeGroup = ScriptableObject.CreateInstance<RecipeGroup>();
            recipeGroup.UniqueID = uniqueID;
            recipeGroup.name = uniqueID;
            recipeGroup.Recipes = recipes;
            return recipeGroup;
        }


        public static List<Recipe> GetAllKnownRecipes()
        {
            var recipes = new List<Recipe>();
            foreach (var recipeGroupIndex in LearnedCraftGroups)
            {
                var recipeGroup = RecipeGroup.RetrieveCraftGroup(recipeGroupIndex);
                if (recipeGroup == null) continue;
                recipes.AddRange(recipeGroup.Recipes);
            }

            return recipes;
        }

        public static List<Recipe> GetAllKnownTTypeRecipes(RecipeType recipeType)
        {
            var recipes = new List<Recipe>();
            foreach (var recipeGroupIndex in LearnedCraftGroups)
            {
                var recipeGroup = RecipeGroup.RetrieveCraftGroup(recipeGroupIndex);
                if (recipeGroup == null) continue;
                if (recipeGroup.RecipeType == recipeType)
                    recipes.AddRange(recipeGroup.Recipes);
            }

            return recipes;
        }

        public static bool IsCraftGroupLearned(string uniqueID)
        {
            return LearnedCraftGroups.Contains(uniqueID);
        }

        public static void SaveLearnedCraftGroup(string uniqueID, bool b)
        {
            ES3.Save(uniqueID, b, GetSaveFilePath());
            LearnedCraftGroups.Add(uniqueID);
        }
        public bool HasSavedData()
        {
            return ES3.KeyExists(_savePath);
        }
    }
}
