using System.Collections.Generic;
using Gameplay.Player.Inventory;
using MoreMountains.Tools;
using Prefabs.UI.PrefabRequiredScripts;
using Project.Gameplay.Events;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

public class PickableItemsListPanel : MonoBehaviour, MMEventListener<ItemEvent>
{
    // 3 sublists
    public GameObject[] itemSubLists;
    // 12 items total in 3 sublists
    public GameObject[] itemDisplayerPanels;

    public GameObject itemInfoPrefab;

    public TMPInventoryDetails ItemDetailsPanel;

    int _currentPageIndex;
    int _pagesCount;

    public List<InventoryItem> CurrentPreviewedItems { get; private set; }

    void Start()
    {
        CurrentPreviewedItems = new List<InventoryItem>();
        _pagesCount = itemSubLists.Length;
        if (ItemDetailsPanel != null)
            ItemDetailsPanel.gameObject.SetActive(false);
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(ItemEvent eventType)
    {
        if (eventType.EventName == "ShowItemInfo")
        {
            Debug.Log("ShowItemInfo event received");
            ItemDetailsPanel.gameObject.SetActive(true);
            ItemDetailsPanel.DisplayPreview(eventType.Item);
        }
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


    public void AddItemToItemsList(InventoryItem item, ManualItemPicker manualItemPicker)
    {
        CurrentPreviewedItems.Add(item);
        TryAddItemToPanel(item, manualItemPicker);
        if (ItemDetailsPanel != null) HideInfoPanel();
    }
    void HideInfoPanel()
    {
        ItemDetailsPanel.HidePreview();
        ItemDetailsPanel.gameObject.SetActive(false);
    }

    void TryAddItemToPanel(InventoryItem item, ManualItemPicker manualItemPicker)
    {
        for (var i = 0; i < itemDisplayerPanels.Length; i++)
            if (itemDisplayerPanels[i].transform.childCount == 0)
            {
                var itemInfo = Instantiate(itemInfoPrefab, itemDisplayerPanels[i].transform);
                itemInfo.GetComponent<ItemInfoPrefab>().SetItem(item, manualItemPicker);
                return;
            }
    }

    public void RemoveItemFromItemsList(InventoryItem item)
    {
        if (CurrentPreviewedItems.Contains(item))
            CurrentPreviewedItems.Remove(item);
        else Debug.LogWarning("Item not found in the list");

        TryRemoveItemFromPanel(item);

        if (ItemDetailsPanel != null)
            HideInfoPanel();
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

        if (ItemDetailsPanel != null)
            HideInfoPanel();
    }
    public void RefreshPreviewOrder()
    {
    }
}
