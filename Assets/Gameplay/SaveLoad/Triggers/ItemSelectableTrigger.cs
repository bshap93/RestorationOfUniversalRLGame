using Gameplay.Player.Inventory;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using Project.Gameplay.Events;
using Project.Gameplay.SaveLoad.Triggers;
using UnityEngine;

namespace Gameplay.SaveLoad.Triggers
{
    public class ItemSelectableTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>, ISelectableTrigger
    {
        public BaseItem Item;

        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks;

        public bool NotPickable;
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
                            Item, NotPickable ? null : GetComponent<ManualItemPicker>());

                        _playerPreviewManager.ShowSelectedItemPreviewPanel();
                    }

                if (!NotPickable)
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

                if (!NotPickable)
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

            _selectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.AddToItemListPreview(Item, manualItemPicker);
            _playerPreviewManager.ShowSelectedItemPreviewPanel();
        }

        public void OnUnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindFirstObjectByType<PlayerItemListPreviewManager>();

            _deselectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.RemoveFromItemListPreview(Item);
            _playerPreviewManager.HidePanelIfEmpty(Item);
        }

        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = FindObjectOfType<PlayerItemListPreviewManager>();

                if (_selectionFeedbacks == null)
                    _selectionFeedbacks = _playerPreviewManager.SelectionFeedbacks;

                if (_deselectionFeedbacks == null)
                    _deselectionFeedbacks = _playerPreviewManager.DeselectionFeedbacks;
            }
        }
    }
}
