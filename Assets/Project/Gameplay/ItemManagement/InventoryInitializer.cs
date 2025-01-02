using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

public class InventoryInitializer : MonoBehaviour
{
    public InventoryItem[] initialItems; // Drag and drop your ScriptableObject items here
    public int[] itemQuantities; // Match quantities with initialItems array
    Inventory _targetInventory; // Reference to the Inventory component

    void Start()
    {
        _targetInventory = GetComponent<Inventory>();
        if (_targetInventory == null || initialItems == null || itemQuantities == null)
        {
            Debug.LogWarning("InventoryInitializer: Missing references.");
            return;
        }

        for (var i = 0; i < initialItems.Length; i++)
            if (initialItems[i] != null)
            {
                var quantity = i < itemQuantities.Length ? itemQuantities[i] : 1;
                _targetInventory.AddItem(initialItems[i], quantity);
            }
    }
}
