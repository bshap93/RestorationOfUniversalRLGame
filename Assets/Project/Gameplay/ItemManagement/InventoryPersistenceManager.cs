using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class InventoryPersistenceManager : MonoBehaviour, MMEventListener<MMGameEvent>
{
    [SerializeField] Inventory mainInventory; // Assign your Main Inventory here

    [SerializeField] Inventory equipmentInventory; // Assign your Equipment Inventory here
    InventoryItem[] _equipmentInventorySavedState;

    InventoryItem[] _mainInventorySavedState;

    void OnEnable()
    {
        // Subscribe to global save/load events
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        // Unsubscribe to prevent leaks
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMGameEvent gameEvent)
    {
        if (gameEvent.EventName == "SaveInventory")
            SaveInventories();
        else if (gameEvent.EventName == "Revert") RevertInventoriesToLastSave();
    }

    void SaveInventories()
    {
        // Save Main Inventory
        _mainInventorySavedState = SaveInventoryState(mainInventory);

        // Save Equipment Inventory
        _equipmentInventorySavedState = SaveInventoryState(equipmentInventory);

        Debug.Log("Inventories saved.");
    }

    void RevertInventoriesToLastSave()
    {
        // Revert Main Inventory
        if (_mainInventorySavedState != null) RevertInventoryState(mainInventory, _mainInventorySavedState);

        // Revert Equipment Inventory
        if (_equipmentInventorySavedState != null)
            RevertInventoryState(equipmentInventory, _equipmentInventorySavedState);

        Debug.Log("Inventories reverted to last saved state.");
    }

    InventoryItem[] SaveInventoryState(Inventory inventory)
    {
        var savedState = new InventoryItem[inventory.Content.Length];
        for (var i = 0; i < inventory.Content.Length; i++)
            if (!InventoryItem.IsNull(inventory.Content[i]))
                savedState[i] = inventory.Content[i].Copy();

        Debug.Log("Inventory state saved.");

        return savedState;
    }

    void RevertInventoryState(Inventory inventory, InventoryItem[] savedState)
    {
        inventory.EmptyInventory();
        for (var i = 0; i < savedState.Length; i++)
            if (!InventoryItem.IsNull(savedState[i]))
                inventory.AddItem(savedState[i].Copy(), savedState[i].Quantity);
    }
}
