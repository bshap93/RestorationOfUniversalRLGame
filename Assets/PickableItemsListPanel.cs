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
    int _pagesCount;

    public List<InventoryItem> CurrentPreviewedItems { get; private set; }

    void Start()
    {
        CurrentPreviewedItems = new List<InventoryItem>();
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

    public void SetToPageNumber(int pageNumber)
    {
        if (pageNumber < 0 || pageNumber >= _pagesCount)
        {
            Debug.LogWarning("Invalid page number");
            return;
        }

        _currentPageIndex = pageNumber;

        for (var i = 0; i < itemSubLists.Length; i++)
            if (i != _currentPageIndex)
                itemSubLists[i].SetActive(false);
            else itemSubLists[i].SetActive(true);
    }


    public void AddItemToItemsList(InventoryItem item)
    {
        CurrentPreviewedItems.Add(item);
        TryAddItemToPanel(item);
    }

    void TryAddItemToPanel(InventoryItem item)
    {
        for (var i = 0; i < itemDisplayerPanels.Length; i++)
            if (itemDisplayerPanels[i].transform.childCount == 0)
            {
                var itemInfo = Instantiate(itemInfoPrefab, itemDisplayerPanels[i].transform);
                itemInfo.GetComponent<ItemInfoPrefab>().SetItem(item);
                return;
            }
    }

    public void RemoveItemFromItemsList(InventoryItem item)
    {
        if (CurrentPreviewedItems.Contains(item))
            CurrentPreviewedItems.Remove(item);
        else Debug.LogWarning("Item not found in the list");

        TryRemoveItemFromPanel(item);
    }

    void TryRemoveItemFromPanel(InventoryItem item)
    {
        for (var i = 0; i < itemDisplayerPanels.Length; i++)
        {
            if (itemDisplayerPanels[i].transform.childCount == 0) continue;
            var itemInfo = itemDisplayerPanels[i].GetComponentInChildren<ItemInfoPrefab>().gameObject;
            if (itemInfo != null && itemInfo.GetComponent<ItemInfoPrefab>().item == item)
            {
                Destroy(itemInfo);
                return;
            }
        }
    }

    public void DisplayPreview()
    {
    }
    public void RemoveAllItemsFromList()
    {
        CurrentPreviewedItems.Clear();
        foreach (var panel in itemDisplayerPanels)
            if (panel.transform.childCount > 0)
                Destroy(panel.transform.GetChild(0).gameObject);
    }
    public void RefreshPreviewOrder()
    {

    }
}
