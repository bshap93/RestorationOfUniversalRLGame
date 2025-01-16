using System.Collections.Generic;
using UnityEngine;

public class PickableManager : MonoBehaviour
{
    public static HashSet<string> PickedItems = new();

    void Awake()
    {
        LoadPickedItems();

        // Don't destroy this object when loading a new scene
        DontDestroyOnLoad(gameObject);
    }

    void LoadPickedItems()
    {
        if (ES3.FileExists("PickedItems.es3"))
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

        Debug.Log("[PickableManager] All picked items reset.");
    }

    public static bool IsItemPicked(string uniqueID)
    {
        return PickedItems.Contains(uniqueID);
    }
}
