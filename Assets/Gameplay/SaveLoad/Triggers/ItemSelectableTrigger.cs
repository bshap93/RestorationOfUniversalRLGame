using Gameplay.Player.Inventory;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Events;
using Project.Gameplay.Interactivity.Food;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.SaveLoad.Triggers
{
    public class ItemSelectableTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>, ISelectableTrigger
    {
        public InventoryItem Item;

        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks;
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
            if (_selectionFeedbacks == null)
            {
                if (Item != null && Item is RawFood)
                    _selectionFeedbacks = GameObject.Find("FoodPicked").GetComponent<MMFeedbacks>();
                else
                    _selectionFeedbacks = GameObject.Find("SelectionFeedbacks").GetComponent<MMFeedbacks>();
            }
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

                var itemPicker = GetComponent<ManualItemPicker>();
                if (itemPicker != null)
                {
                    itemPicker.SetInRange(true);
                    ItemEvent.Trigger("ItemPickupRangeEntered", Item, transform);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = other.GetComponent<PlayerItemListPreviewManager>();

                var itemPicker = GetComponent<ManualItemPicker>();
                if (itemPicker != null)
                {
                    itemPicker.SetInRange(false);
                    ItemEvent.Trigger("ItemPickupRangeExited", Item, transform);
                }
            }
        }

        public void OnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindObjectOfType<PlayerItemListPreviewManager>();

            _selectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.ShowSelectedItemPreviewPanel(Item);
        }

        public void OnUnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindObjectOfType<PlayerItemListPreviewManager>();

            _deselectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.HideSelectedItemPreviewPanel();
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
