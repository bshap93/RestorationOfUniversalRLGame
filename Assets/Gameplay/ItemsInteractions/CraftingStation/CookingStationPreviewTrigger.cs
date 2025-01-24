using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.Player.Interaction;
using UnityEngine;

namespace Gameplay.ItemsInteractions.CraftingStation
{
    public class CookingStationPreviewTrigger : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        public CookingStation CookingStation;

        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks;

        ManualCraftingStationInteract _craftingStationInteract;
        bool _isInPlayerRange;
        CookingStationPreviewManager _playerPreviewManager;

        void Awake()
        {
            InitializeCraftingStationInteract();
        }

        void Start()
        {
            // Try to find the preview manager if not already set
            if (_playerPreviewManager == null)
            {
                _playerPreviewManager = FindObjectOfType<CookingStationPreviewManager>();
                if (_playerPreviewManager == null)
                    Debug.LogWarning($"[{gameObject.name}] CraftingStationPreviewManager not found in scene");
            }

            // Initialize feedbacks from preview manager if not set
            if (_selectionFeedbacks == null && _playerPreviewManager != null)
                _selectionFeedbacks = _playerPreviewManager.SelectionFeedbacks;

            if (_deselectionFeedbacks == null && _playerPreviewManager != null)
                _deselectionFeedbacks = _playerPreviewManager.DeselectionFeedbacks;
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
            if (_isInPlayerRange)
                // Make sure to clean up if disabled while player is in range
                HandlePlayerExit();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player")) HandlePlayerEnter(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player")) HandlePlayerExit();
        }

        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_playerPreviewManager == null)
                    _playerPreviewManager = FindObjectOfType<CookingStationPreviewManager>();

                if (_selectionFeedbacks == null) _selectionFeedbacks = _playerPreviewManager?.SelectionFeedbacks;

                if (_deselectionFeedbacks == null) _deselectionFeedbacks = _playerPreviewManager?.DeselectionFeedbacks;
            }
        }

        void InitializeCraftingStationInteract()
        {
            _craftingStationInteract = GetComponent<ManualCraftingStationInteract>();

            if (_craftingStationInteract == null)
            {
                _craftingStationInteract = gameObject.AddComponent<ManualCraftingStationInteract>();
                Debug.Log($"[{gameObject.name}] Added ManualCraftingStationInteract component");
            }

            if (CookingStation == null)
            {
                Debug.LogError($"[{gameObject.name}] CraftingStation reference is missing!");
                return;
            }

            _craftingStationInteract.cookingStation = CookingStation;
            Debug.Log($"[{gameObject.name}] Initialized with CraftingStation: {CookingStation.CraftingStationName}");
        }

        void HandlePlayerEnter(Collider playerCollider)
        {
            if (_craftingStationInteract == null || CookingStation == null)
            {
                Debug.LogError($"[{gameObject.name}] Missing required components for player interaction");
                return;
            }

            // Get the preview manager from the player if we don't have it
            if (_playerPreviewManager == null)
            {
                _playerPreviewManager = playerCollider.GetComponent<CookingStationPreviewManager>();
                if (_playerPreviewManager == null)
                {
                    Debug.LogError($"[{gameObject.name}] Player is missing CraftingStationPreviewManager component");
                    return;
                }
            }

            _isInPlayerRange = true;
            _craftingStationInteract.SetInRange(true);

            MMGameEvent.Trigger(
                "CraftingStationRangeEntered",
                stringParameter: CookingStation.CraftingStationId,
                vector3Parameter: transform.position
            );

            Debug.Log(
                $"[{gameObject.name}] Player entered range of crafting station: {CookingStation.CraftingStationName}");
        }

        void HandlePlayerExit()
        {
            if (_craftingStationInteract != null && CookingStation != null)
            {
                _isInPlayerRange = false;
                _craftingStationInteract.SetInRange(false);

                MMGameEvent.Trigger(
                    "CraftingStationRangeExited",
                    stringParameter: CookingStation.CraftingStationId,
                    vector3Parameter: transform.position
                );

                Debug.Log(
                    $"[{gameObject.name}] Player exited range of crafting station: {CookingStation.CraftingStationName}");
            }
        }

        public void OnSelectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindObjectOfType<CookingStationPreviewManager>();

            _selectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager?.ShowSelectedCraftingStationPreviewPanel(CookingStation);
            Debug.Log($"[{gameObject.name}] Selected crafting station: {CookingStation.CraftingStationName}");
        }

        public void OnDeselectedItem()
        {
            if (_playerPreviewManager == null)
                _playerPreviewManager = FindObjectOfType<CookingStationPreviewManager>();

            _deselectionFeedbacks?.PlayFeedbacks();
            _playerPreviewManager?.HideSelectedCraftingStationPreviewPanel();
            Debug.Log($"[{gameObject.name}] Deselected crafting station: {CookingStation.CraftingStationName}");
        }
    }
}
