using System.Collections.Generic;
using UnityEngine;

public static class DispenserManagerDebug
{
    [ContextMenu("Reset Dispenser States")]
    public static void ResetDispenserStatesMenu()
    {
        DispenserManager.ResetDispenserStates();
    }
}

public class DispenserManager : MonoBehaviour
{
    public static Dictionary<string, int> DispenserStates = new();

    void Awake()
    {
        LoadDispenserStates();
        DontDestroyOnLoad(gameObject); // Keep it across scenes
    }

    public static void ResetDispenserStates()
    {
        ES3.DeleteFile("DispenserStates.es3");
        DispenserStates.Clear();
        Debug.Log("All dispenser states reset.");
    }


    void LoadDispenserStates()
    {
        if (ES3.FileExists("DispenserStates.es3"))
        {
            var keys = ES3.GetKeys("DispenserStates.es3");
            foreach (var key in keys)
            {
                var remaining = ES3.Load<int>(key, "DispenserStates.es3");
                DispenserStates[key] = remaining;
            }
        }
    }

    public static void SaveDispenserState(string uniqueID, int remaining)
    {
        DispenserStates[uniqueID] = remaining;
        ES3.Save(uniqueID, remaining, "DispenserStates.es3");
    }

    public static int GetSavedSupply(string uniqueID)
    {
        return DispenserStates.ContainsKey(uniqueID) ? DispenserStates[uniqueID] : -1;
    }
}
