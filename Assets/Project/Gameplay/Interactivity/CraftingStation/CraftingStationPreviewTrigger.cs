using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Player.Interaction;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class CraftingStationPreviewTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        public CraftingStation CraftingStation;

        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks;
        ManualCraftingStationInteract _craftingStationInteract;

        PlayerInteractionPreviewManager _playerPreviewManager;

        void Awake()
        {
            _craftingStationInteract = GetComponent<ManualCraftingStationInteract>();

            if (_craftingStationInteract == null)
                _craftingStationInteract = gameObject.AddComponent<ManualCraftingStationInteract>();

            _craftingStationInteract.CraftingStation = CraftingStation;
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
                    _playerPreviewManager = other.GetComponent<PlayerInteractionPreviewManager>();

                var itemPicker = GetComponent<ManualCraftingStationInteract>();
                if (itemPicker != null)
                {
                    itemPicker.SetInRange(true);
                    MMGameEvent.Trigger(
                        "CraftingStationRangeEntered",
                        stringParameter: CraftingStation.CraftingStationId, vector3Parameter: transform.position);
                }
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var itemPicker = GetComponent<ManualCraftingStationInteract>();
                if (itemPicker != null)
                {
                    itemPicker.SetInRange(false);
                    MMGameEvent.Trigger(
                        "CraftingStationRangeExited",
                        stringParameter: CraftingStation.CraftingStationId, vector3Parameter: transform.position);
                }
            }
        }
        public void OnMMEvent(MMCameraEvent eventType)
        {
            if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = FindObjectOfType<PlayerInteractionPreviewManager>();

                if (_selectionFeedbacks == null)
                    _selectionFeedbacks = _playerPreviewManager.SelectionFeedbacks;

                if (_deselectionFeedbacks == null)
                    _deselectionFeedbacks = _playerPreviewManager.DeselectionFeedbacks;
            }
        }

        public void OnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindObjectOfType<PlayerInteractionPreviewManager>();

            _selectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.ShowSelectedInteractablePreviewPanel(CraftingStation);
        }

        public void OnDeselectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindObjectOfType<PlayerInteractionPreviewManager>();

            _deselectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager.HideSelectedInteractablePreviewPanel();
        }
    }
}
