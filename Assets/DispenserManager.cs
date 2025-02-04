using System.Collections.Generic;
using Core.SaveSystem;
using UnityEngine;

public static class DispenserManagerDebug
{
    [ContextMenu("Reset Dispenser States")]
    public static void ResetDispenserStatesMenu()
    {
        DispenserManager.ResetDispenserStates();
    }
}

public class DispenserManager : PersistentManager
{
    public static Dictionary<string, int> DispenserStates = new();

    protected override void OnManagerAwake()
    {
        LoadDispenserStates();
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
