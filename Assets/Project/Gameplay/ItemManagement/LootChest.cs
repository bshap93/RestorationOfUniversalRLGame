using System.Collections.Generic;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using Project.Gameplay.ItemManagement;
using Project.Gameplay.Player.Inventory;
using Project.UI.HUD;
using UnityEngine;

public class LootChest : MonoBehaviour
{
    public GameObject itemPrefab;

    public List<InventoryItem> items;

    public int maxItems;

    public List<Transform> itemSlots;

    public GameObject chestLid;
    
    private bool isOpen = false;

    bool _isInRange;
    
    [SerializeField] MMFeedbacks openChestFeedbacks;


    List<GameObject> _itemInstances;

    PromptManager _promptManager;
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

            var itemPreviewTrigger = itemInstance.GetComponent<ItemPreviewTrigger>();
            itemPreviewTrigger.Item = item;

            itemSlot.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInRange = true;
            _promptManager?.ShowInteractPrompt("Press F to Open Chest");
        }
    }
     
    void Update()
    {
        if (_isInRange && UnityEngine.Input.GetKeyDown(KeyCode.F)) OpenChest();
    }
    
    void OpenChest()
    {
        if (isOpen) return;
        chestLid.SetActive(false);
        foreach (var itemSlot in itemSlots)
        {
            itemSlot.gameObject.SetActive(true);
        }
        openChestFeedbacks?.PlayFeedbacks();
        _promptManager?.HideInteractPrompt();
        isOpen = true;
    }
}
