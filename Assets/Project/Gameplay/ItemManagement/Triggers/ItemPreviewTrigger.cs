using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Player.Inventory;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class ItemPreviewTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        public InventoryItem Item; // Assign the InventoryItem to display

        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks;

        PlayerItemPreviewManager _previewManager;

        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening();
        }


        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_previewManager == null)
                    _previewManager = other.GetComponent<PlayerItemPreviewManager>();

                _previewManager.RegisterItem(Item);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (_previewManager == null)
                    _previewManager = other.GetComponent<PlayerItemPreviewManager>();

                _previewManager.UnregisterItem(Item);
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
            {
                _previewManager = FindObjectOfType<PlayerItemPreviewManager>();
                Debug.LogWarning("PreviewManager not found in the scene.");
            }

            _selectionFeedbacks?.PlayFeedbacks();


            _previewManager.RegisterItem(Item);
            _previewManager.ShowSelectedItemPreviewPanel(Item);
        }

        public void OnUnSelectedItem()
        {
            if (_previewManager == null)
                _previewManager = FindObjectOfType<PlayerItemPreviewManager>();

            _deselectionFeedbacks?.PlayFeedbacks();

            _previewManager.UnregisterItem(Item);
            _previewManager.HideSelectedItemPreviewPanel();
        }
    }
}
