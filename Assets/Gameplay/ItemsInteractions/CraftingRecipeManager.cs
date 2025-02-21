#if UNITY_EDITOR
using System.Collections.Generic;
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

        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _savePath = GetSaveFilePath();
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

        public static bool IsCraftGroupLearned(string uniqueID)
        {
            return LearnedCraftGroups.Contains(uniqueID);
        }

        public static void SaveLearnedCraftGroup(string uniqueID, bool b)
        {
            ES3.Save(uniqueID, b, GetSaveFilePath());
        }
    }
}
