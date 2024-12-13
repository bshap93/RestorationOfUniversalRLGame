using System;
using System.Collections.Generic;
using Project.Gameplay.ItemManagement;
using UnityEngine;

public class InventoryHotbarHotkeyManager : MonoBehaviour
{
    public string[] HotbarKeys = { "1", "2", "3", "4" };
    public string[] HotbarAltKeys = { "h", "j", "k", "l" };
    CustomInventoryHotbar _customInventoryHotbar;
    Dictionary<KeyCode, int> _keyMappings;

    void Start()
    {
        _customInventoryHotbar = FindObjectOfType<CustomInventoryHotbar>();

        // Initialize key mappings for faster lookups
        _keyMappings = new Dictionary<KeyCode, int>();
        for (var i = 0; i < HotbarKeys.Length; i++)
        {
            var primaryKey = (KeyCode)Enum.Parse(typeof(KeyCode), HotbarKeys[i].ToUpper());
            var altKey = (KeyCode)Enum.Parse(typeof(KeyCode), HotbarAltKeys[i].ToUpper());

            if (!_keyMappings.ContainsKey(primaryKey)) _keyMappings[primaryKey] = i;
            if (!_keyMappings.ContainsKey(altKey)) _keyMappings[altKey] = i;
        }
    }

    void Update()
    {
        if (!Input.anyKeyDown) return; // Early exit if no key was pressed

        foreach (var key in _keyMappings.Keys)
            if (Input.GetKeyDown(key))
            {
                _customInventoryHotbar.Action(_keyMappings[key]);
                break; // Exit early after handling the key press
            }
    }
}
