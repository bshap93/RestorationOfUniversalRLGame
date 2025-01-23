using System.Collections.Generic;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

public class PickableItemsListPanel : MonoBehaviour
{
    // 3 sublists
    public GameObject[] itemSubLists;
    // 12 items total in 3 sublists
    public GameObject[] itemDisplayerPanels;
    public GameObject itemInfoPrefab;
    List<InventoryItem> _currentPreviewedItems;


    public void AddItemToItemsList(InventoryItem item)
    {
        _currentPreviewedItems.Add(item);
    }

    public void RemoveItemFromItemsList(InventoryItem item)
    {
        if (_currentPreviewedItems.Contains(item))
            _currentPreviewedItems.Remove(item);
        else Debug.LogWarning("Item not found in the list");
    }

    public void DisplayPreview()
    {
        Debug.Log("Logging from DisplayPreview method");
    }
}
