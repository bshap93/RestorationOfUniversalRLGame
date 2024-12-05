using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

public class LootChest : MonoBehaviour
{
    public GameObject itemPrefab;

    public List<InventoryItem> items;

    public int maxItems;

    public List<Transform> itemSlots;

    List<GameObject> _itemInstances;
    // Start is called before the first frame update
    void Start()
    {
        for (var i = 0; i < items.Count; i++)
        {
            if (i >= maxItems) break;

            var item = items[i];
            var itemSlot = itemSlots[i];
            var itemInstance = Instantiate(itemPrefab, itemSlot.position, Quaternion.identity);
            itemInstance.transform.SetParent(itemSlot);
            var itemPicker = itemInstance.GetComponent<ManualItemPicker>();
            itemPicker.Item = item;
            itemPicker.pickedMmFeedbacks = itemInstance.GetComponentInChildren<MMF_Player>();

            var itemPreviewTrigger = itemInstance.GetComponent<ItemPreviewTrigger>();
            itemPreviewTrigger.Item = item;

            itemSlot.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
