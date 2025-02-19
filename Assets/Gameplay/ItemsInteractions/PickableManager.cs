#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public static class PickableManagerDebug
    {
        [MenuItem("Debug/Reset Picked Items")]
        public static void ResetPickedItemsMenu()
        {
            PickableManager.ResetPickedItems();
        }
    }
#endif


    public class PickableManager : MonoBehaviour
    {
        public static HashSet<string> PickedItems = new();

        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _savePath = GetSaveFilePath();
            LoadPickedItems();
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "PickedItems.es3" : $"{slotPath}/PickedItems.es3";
        }

        public void LoadPickedItems()
        {
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(_savePath);
            if (exists)
            {
                var keys = ES3.GetKeys(_savePath);
                foreach (var key in keys)
                    if (ES3.Load<bool>(key, _savePath))
                        PickedItems.Add(key);
            }
        }

        public static void ResetPickedItems()
        {
            var saveFilePath = GetSaveFilePath();
            // Delete the Easy Save file storing picked items
            ES3.DeleteFile(GetSaveFilePath());

            // Clear the in-memory picked items list (if used)
            PickedItems.Clear();
        }


        public static bool IsItemPicked(string uniqueID)
        {
            return PickedItems.Contains(uniqueID);
        }
        public static void SavePickedItem(string uniqueID, bool b)
        {
            ES3.Save(uniqueID, b, GetSaveFilePath());
        }
        public static void SaveItemPosition(string itemPickerUniqueID, Vector3 transformPosition, string prefabName)
        {
            throw new NotImplementedException();
        }
    }
}
