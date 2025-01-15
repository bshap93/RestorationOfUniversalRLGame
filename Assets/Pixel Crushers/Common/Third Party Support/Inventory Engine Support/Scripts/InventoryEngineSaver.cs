// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using System.Reflection;
using MoreMountains.Tools;
using MoreMountains.InventoryEngine;

namespace PixelCrushers.InventoryEngineSupport
{

    /// <summary>
    /// Pixel Crushers Saver for an Inventory Engine inventory.
    /// </summary>
    [AddComponentMenu("Pixel Crushers/Save System/Savers/Inventory Engine/Inventory Engine Saver")]
    [RequireComponent(typeof(MoreMountains.InventoryEngine.Inventory))]
    public class InventoryEngineSaver : Saver
    {

        public string playerID = string.Empty;

        public override string RecordData()
        {
            // Inventory.FillSerializedInventory is protected, so we need to use reflection:
            var serializedInventory = new SerializedInventory();
            var method = typeof(Inventory).GetMethod("FillSerializedInventory", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(GetComponent<MoreMountains.InventoryEngine.Inventory>(), new object[] { serializedInventory });
            return SaveSystem.Serialize(serializedInventory);
        }

        public override void ApplyData(string s)
        {
            if (string.IsNullOrEmpty(s)) return;
            var serializedInventory = SaveSystem.Deserialize<SerializedInventory>(s);
            // JSON converts nulls to empty strings. Need to convert back to nulls:
            for (int i = 0; i < serializedInventory.ContentType.Length; i++)
            {
                if (string.IsNullOrEmpty(serializedInventory.ContentType[i]))
                {
                    serializedInventory.ContentType[i] = null;
                }
            }
            // Inventory.ExtractSerializedInventory is protected, so we need to use reflection:
            var method = typeof(Inventory).GetMethod("ExtractSerializedInventory", BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(GetComponent<MoreMountains.InventoryEngine.Inventory>(), new object[] { serializedInventory });
            MMEventManager.TriggerEvent(new MMInventoryEvent(MMInventoryEventType.InventoryLoaded, null, this.name, null, 0, 0, playerID));
        }

    }
}