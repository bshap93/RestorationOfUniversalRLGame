using Gameplay.Player.Inventory;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Events;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.SaveLoad.Triggers
{
    public class ItemSelectableTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>, ISelectableTrigger
    {
        public InventoryItem Item;

        [FormerlySerializedAs("_selectionFeedbacks")] [SerializeField]
        MMFeedbacks selectionFeedbacks;
        [FormerlySerializedAs("_deselectionFeedbacks")] [SerializeField]
        MMFeedbacks deselectionFeedbacks;

        [FormerlySerializedAs("NotPickable")] public bool notPickable;
        ManualItemPicker _itemPicker;

        PlayerItemListPreviewManager _playerPreviewManager;


        void Awake()
        {
            _itemPicker = GetComponent<ManualItemPicker>();


            if (_itemPicker == null)
                _itemPicker = gameObject.AddComponent<ManualItemPicker>();

            _itemPicker.Item = Item;
        }

        void Start()
        {
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = other.GetComponent<PlayerItemListPreviewManager>();

                if (_playerPreviewManager != null && Item != null)
                    if (!_playerPreviewManager.CurrentPreviewedItems.Contains(Item))
                    {
                        _playerPreviewManager.AddToItemListPreview(
                            Item, notPickable ? null : GetComponent<ManualItemPicker>());

                        _playerPreviewManager.ShowSelectedItemPreviewPanel();
                    }

                if (!notPickable)
                {
                    // Handle normal pickable logic
                    var itemPicker = GetComponent<ManualItemPicker>();
                    if (itemPicker != null)
                    {
                        itemPicker.SetInRange(true);
                        ItemEvent.Trigger("ItemPickupRangeEntered", Item, transform);
                    }
                }
            }
        }


        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = other.GetComponent<PlayerItemListPreviewManager>();

                if (_playerPreviewManager != null && Item != null)
                {
                    // Remove the item from the preview list
                    _playerPreviewManager.RemoveFromItemListPreview(Item);
                    _playerPreviewManager.HidePanelIfEmpty(Item);
                }

                if (!notPickable)
                {
                    // Handle normal pickable logic
                    var itemPicker = GetComponent<ManualItemPicker>();
                    if (itemPicker != null)
                    {
                        itemPicker.SetInRange(false);
                        ItemEvent.Trigger("ItemPickupRangeExited", Item, transform);
                    }
                }
            }
        }

        public void OnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindFirstObjectByType<PlayerItemListPreviewManager>();

            var manualItemPicker = GetComponent<ManualItemPicker>();

            selectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.AddToItemListPreview(Item, manualItemPicker);
            _playerPreviewManager.ShowSelectedItemPreviewPanel();
        }

        public void OnUnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindFirstObjectByType<PlayerItemListPreviewManager>();

            deselectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.RemoveFromItemListPreview(Item);
            _playerPreviewManager.HidePanelIfEmpty(Item);
        }

        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = FindObjectOfType<PlayerItemListPreviewManager>();

                if (selectionFeedbacks == null)
                    selectionFeedbacks = _playerPreviewManager.selectionFeedbacks;

                if (deselectionFeedbacks == null)
                    deselectionFeedbacks = _playerPreviewManager.deselectionFeedbacks;
            }
        }
    }
}
