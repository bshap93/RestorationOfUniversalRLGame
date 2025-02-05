#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        LoadPickedItems();
    }

    public void LoadPickedItems()
    {
        var exists = ES3.FileExists("PickedItems.es3");
        if (exists)
        {
            var keys = ES3.GetKeys("PickedItems.es3");
            foreach (var key in keys)
                if (ES3.Load<bool>(key, "PickedItems.es3"))
                    PickedItems.Add(key);
        }
    }

    public static void ResetPickedItems()
    {
        // Delete the Easy Save file storing picked items
        ES3.DeleteFile("PickedItems.es3");

        // Clear the in-memory picked items list (if used)
        PickedItems.Clear();
    }


    public static bool IsItemPicked(string uniqueID)
    {
        return PickedItems.Contains(uniqueID);
    }
    public static void SavePickedItem(string uniqueID, bool b)
    {
        ES3.Save(uniqueID, b, "PickedItems.es3");
    }
    public static void SaveItemPosition(string itemPickerUniqueID, Vector3 transformPosition, string prefabName)
    {
        throw new NotImplementedException();
    }
}
