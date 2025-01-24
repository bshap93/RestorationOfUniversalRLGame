using System.Collections.Generic;
using Gameplay.Player.Inventory;
using MoreMountains.Feedbacks;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.SaveLoad.Triggers;
using Project.UI.HUD;
using UnityEngine;

public class LootChest : MonoBehaviour
{
    public GameObject itemPrefab;

    public List<InventoryItem> items;

    public int maxItems;

    public List<Transform> itemSlots;

    public GameObject chestLid;

    [SerializeField] MMFeedbacks openChestFeedbacks;

    bool _isInRange;


    List<GameObject> _itemInstances;

    PromptManager _promptManager;

    bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        _promptManager = FindObjectOfType<PromptManager>();

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

            var itemPreviewTrigger = itemInstance.GetComponent<ItemSelectableTrigger>();
            itemPreviewTrigger.Item = item;

            itemSlot.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (_isInRange && Input.GetKeyDown(KeyCode.F)) OpenChest();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            _isInRange = true;
            _promptManager?.ShowInteractPrompt("Press F to Open Chest");
        }
    }

    void OpenChest()
    {
        if (isOpen) return;
        chestLid.SetActive(false);
        foreach (var itemSlot in itemSlots) itemSlot.gameObject.SetActive(true);
        openChestFeedbacks?.PlayFeedbacks();
        _promptManager?.HideInteractPrompt();
        isOpen = true;
    }
}
