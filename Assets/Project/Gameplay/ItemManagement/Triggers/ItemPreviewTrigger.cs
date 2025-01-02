using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Events;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class ItemPreviewTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        public InventoryItem Item;

        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks;
        ManualItemPicker _itemPicker;

        PlayerItemPreviewManager _previewManager;


        void Awake()
        {
            _itemPicker = GetComponent<ManualItemPicker>();

            if (_itemPicker == null)
                _itemPicker = gameObject.AddComponent<ManualItemPicker>();

            _itemPicker.Item = Item;
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
                if (_previewManager == null)
                    _previewManager = other.GetComponent<PlayerItemPreviewManager>();

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
                if (_previewManager == null)
                    _previewManager = other.GetComponent<PlayerItemPreviewManager>();

                var itemPicker = GetComponent<ManualItemPicker>();
                if (itemPicker != null)
                {
                    itemPicker.SetInRange(false);
                    ItemEvent.Trigger("ItemPickupRangeExited", Item, transform);
                }
            }
        }

        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_previewManager == null)
                    _previewManager = FindObjectOfType<PlayerItemPreviewManager>();

                if (_selectionFeedbacks == null)
                    _selectionFeedbacks = _previewManager.SelectionFeedbacks;

                if (_deselectionFeedbacks == null)
                    _deselectionFeedbacks = _previewManager.DeselectionFeedbacks;
            }
        }

        public void OnSelectedItem()
        {
            if (_previewManager == null)
                _previewManager = FindObjectOfType<PlayerItemPreviewManager>();

            _selectionFeedbacks?.PlayFeedbacks();
            _previewManager.ShowSelectedItemPreviewPanel(Item);
        }

        public void OnUnSelectedItem()
        {
            if (_previewManager == null)
                _previewManager = FindObjectOfType<PlayerItemPreviewManager>();

            _deselectionFeedbacks?.PlayFeedbacks();
            _previewManager.HideSelectedItemPreviewPanel();
        }
    }
}