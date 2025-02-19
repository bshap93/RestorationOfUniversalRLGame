#if UNITY_EDITOR
using System.Collections.Generic;
using Gameplay.ItemsInteractions;
using UnityEditor;
using UnityEngine;

public static class DestructibleManagerDebug
{
    [MenuItem("Debug/Reset Picked Items")]
    public static void ResetPickedItemsMenu()
    {
        DestructibleManager.ResetDestroyedObjects();
    }
}
#endif

namespace Gameplay.ItemsInteractions
{
    public class DestructibleManager : MonoBehaviour
    {
        public static HashSet<string> DestroyedObjects = new();

        string _savePath;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        void Start()
        {
            _savePath = GetSaveFilePath();
            LoadDestroyedObjects();
        }

        static string GetSaveFilePath()
        {
            var slotPath = ES3SlotManager.selectedSlotPath;
            return string.IsNullOrEmpty(slotPath) ? "DestroyedObjects.es3" : $"{slotPath}/DestroyedObjects.es3";
        }

        public void LoadDestroyedObjects()
        {
            var saveFilePath = GetSaveFilePath();
            var exists = ES3.FileExists(_savePath);
            if (exists)
            {
                var keys = ES3.GetKeys(_savePath);
                foreach (var key in keys)
                    if (ES3.Load<bool>(key, _savePath))
                        DestroyedObjects.Add(key);
            }
        }
        public static void ResetDestroyedObjects()
        {
            var saveFilePath = GetSaveFilePath();

            ES3.DeleteFile(GetSaveFilePath());

            DestroyedObjects.Clear();
        }

        public static bool IsObjectDestroyed(string uniqueID)
        {
            return DestroyedObjects.Contains(uniqueID);
        }

        public static void SaveDestroyedObject(string uniqueID, bool b)
        {
            ES3.Save(uniqueID, b, GetSaveFilePath());
            Debug.Log("Saved destroyed object: " + uniqueID);
        }
    }
}
