using System;
using System.Collections.Generic;
using Gameplay.ItemManagement;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.InventoryDisplays;
using UnityEngine;

public class InventoryHotbarHotkeyManager : MonoBehaviour, MMEventListener<MMCameraEvent>
{
    // public string[] HotbarKeys = { "1", "2", "3", "4" };
    public string[] HotbarAltKeys = { "h", "j", "k", "l" };
    CustomCharacterInventory _customCharacterInventory;
    CustomInventoryHotbar _customInventoryHotbar;
    Dictionary<KeyCode, int> _keyMappings;

    void Start()
    {
        _customInventoryHotbar = FindObjectOfType<CustomInventoryHotbar>();

        // Initialize key mappings for faster lookups
        _keyMappings = new Dictionary<KeyCode, int>();
        for (var i = 0; i < HotbarAltKeys.Length; i++)
        {
            // var primaryKey = (KeyCode)Enum.Parse(typeof(KeyCode), HotbarKeys[i].ToUpper());
            var altKey = (KeyCode)Enum.Parse(typeof(KeyCode), HotbarAltKeys[i].ToUpper());

            // if (!_keyMappings.ContainsKey(primaryKey)) _keyMappings[primaryKey] = i;
            if (!_keyMappings.ContainsKey(altKey)) _keyMappings[altKey] = i;
        }
    }

    void Update()
    {
        if (!Input.anyKeyDown) return;

        foreach (var key in _keyMappings.Keys)
            if (Input.GetKeyDown(key))
            {
                _customInventoryHotbar.Action(_keyMappings[key]);
                // Equip weapon from selected hotbar slot
                if (_customCharacterInventory == null)
                {
                    Debug.LogWarning("No CustomCharacterInventory found.");
                    return;
                }

                _customCharacterInventory.EquipWeapon(
                    _customInventoryHotbar.TargetInventory.Content[_keyMappings[key]].ItemID);

                break;
            }
    }

    public void OnMMEvent(MMCameraEvent mmEvent)
    {
        if (mmEvent.EventType != MMCameraEventTypes.SetTargetCharacter)
            _customCharacterInventory = mmEvent.TargetCharacter.gameObject.GetComponent<CustomCharacterInventory>();
    }
}
