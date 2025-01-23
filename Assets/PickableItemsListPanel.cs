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

    int _currentPageIndex;
    List<InventoryItem> _currentPreviewedItems;
    int _pagesCount;

    void Start()
    {
        _currentPreviewedItems = new List<InventoryItem>();
        _pagesCount = itemSubLists.Length;
    }

    public void NextPage()
    {
        _currentPageIndex++;
        if (_currentPageIndex >= _pagesCount)
            _currentPageIndex = 0;

        for (var i = 0; i < itemSubLists.Length; i++)
            if (i != _currentPageIndex)
                itemSubLists[i].SetActive(false);
            else itemSubLists[i].SetActive(true);
    }

    public void PreviousPage()
    {
        _currentPageIndex--;
        if (_currentPageIndex < 0)
            _currentPageIndex = _pagesCount - 1;

        for (var i = 0; i < itemSubLists.Length; i++)
            if (i != _currentPageIndex)
                itemSubLists[i].SetActive(false);
            else itemSubLists[i].SetActive(true);
    }


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
