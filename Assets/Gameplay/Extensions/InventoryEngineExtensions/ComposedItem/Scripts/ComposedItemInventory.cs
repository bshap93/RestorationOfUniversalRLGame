using System;
using System.Linq;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;

public class ComposedItemInventory : Inventory
{
    public override void OnMMEvent(MMInventoryEvent mmEvent)
    {
        if (mmEvent.TargetInventoryName != name || mmEvent.PlayerID != PlayerID) return;
        if (mmEvent.InventoryEventType == MMInventoryEventType.Pick)
            (mmEvent.EventItem as IInitializable)?.Initialize();

        base.OnMMEvent(mmEvent);
    }
    public override void SaveInventory()
    {
        var serializedInventory = new ExtendedSerializedInventory();
        FillSerializedInventory(serializedInventory);
        MMSaveLoadManager.Save(serializedInventory, DetermineSaveName(), _saveFolderName);
    }
    public override void LoadSavedInventory()
    {
        var serializedInventory = (ExtendedSerializedInventory)MMSaveLoadManager.Load(
            typeof(ExtendedSerializedInventory), DetermineSaveName(), _saveFolderName);

        ExtractSerializedInventory(serializedInventory);
        MMInventoryEvent.Trigger(MMInventoryEventType.InventoryLoaded, null, name, null, 0, 0, PlayerID);
    }
    protected override void FillSerializedInventory(SerializedInventory serializedInventory)
    {
        base.FillSerializedInventory(serializedInventory);
        var inventory = (ExtendedSerializedInventory)serializedInventory;
        inventory.ContentSaveData = Content.Select(item => (item as IJsonSerializable)?.Save()).ToArray();
    }
    protected override void ExtractSerializedInventory(SerializedInventory serializedInventory)
    {
        base.ExtractSerializedInventory(serializedInventory);
        if (serializedInventory == null) return;
        var inventory = (ExtendedSerializedInventory)serializedInventory;
        foreach (var pair in Content.Zip(inventory.ContentSaveData, (item, saveData) => (item, saveData)))
            (pair.item as IJsonSerializable)?.Load(pair.saveData);
    }

    [Serializable]
    class ExtendedSerializedInventory : SerializedInventory
    {
        public string[] ContentSaveData;
    }
}
