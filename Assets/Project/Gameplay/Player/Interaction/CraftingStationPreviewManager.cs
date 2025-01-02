using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.UI.HUD;
using UnityEngine;

namespace Project.Gameplay.Player.Interaction
{
    public class CraftingStationPreviewManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public GameObject PreviewPanelUI;

        public MMFeedbacks SelectionFeedbacks;
        public MMFeedbacks DeselectionFeedbacks;
        readonly Dictionary<string, ManualCraftingStationInteract> _craftingStationsInRange = new();
        readonly float _interactCooldown = 0.5f; // Add cooldown to prevent rapid interactions
        readonly object _interactLock = new();

        string _currentCraftingStationId;

        HighlightManager _highlightManager;
        bool _isInteracting;

        bool _isInteractLocked;

        bool _isSorting;
        float _lastInteractTime;
        PreviewManager _previewManager;

        public ManualCraftingStationInteract CurrentPreviewedStationInteract { get; }
        public Interactable currentInteractable { get; }

        void Start()
        {
            _previewManager = FindObjectOfType<PreviewManager>();
            _highlightManager = FindObjectOfType<HighlightManager>();

            if (_previewManager == null) Debug.LogWarning("PreviewManager not found in the scene.");
            if (_highlightManager == null) Debug.LogWarning("HighlightManager not found in the scene.");
        }

        void Update()
        {
            if (!_isSorting) UpdateNearestCraftingStation();
        }

        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening for both MMGameEvent and MMCameraEvent
            this.MMEventStopListening();
        }


        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "CraftingStationRangeEntered")
            {
                string craftingStationId = eventType.StringParameter;
                Vector3 position = eventType.Vector3Parameter;

                // Retrieve the CraftingStation instance (You need a mapping mechanism)
                var craftingStationInteract = FindCraftingStationById(craftingStationId);
                if (craftingStationInteract != null)
                {
                    HandleCraftingStationEntered(craftingStationInteract, position);
                }
            }
            else if (eventType.EventName == "CraftingStationRangeExited")
            {
                string craftingStationId = eventType.StringParameter;

                var craftingStationInteract = FindCraftingStationById(craftingStationId);
                if (craftingStationInteract != null)
                {
                    HandleCraftingStationExited(craftingStationInteract);
                }
            }
        }
        
        void HandleCraftingStationEntered(ManualCraftingStationInteract craftingStationInteract, Vector3 position)
        {
            if (!_craftingStationsInRange.ContainsKey(craftingStationInteract.UniqueID))
            {
                _craftingStationsInRange.Add(craftingStationInteract.UniqueID, craftingStationInteract);
                _highlightManager.SelectObject(craftingStationInteract.transform);

                // Show preview for the first/only station
                if (_craftingStationsInRange.Count == 1)
                {
                    ShowPreviewPanel(craftingStationInteract.craftingStation);
                }

                UpdateNearestCraftingStation();
            }
        }

        void HandleCraftingStationExited(ManualCraftingStationInteract craftingStationInteract)
        {
            if (_craftingStationsInRange.ContainsKey(craftingStationInteract.UniqueID))
            {
                _craftingStationsInRange.Remove(craftingStationInteract.UniqueID);
                _highlightManager.UnselectObject(craftingStationInteract.transform);

                // If no stations remain, clear the preview
                if (_craftingStationsInRange.Count == 0)
                {
                    HidePreviewPanel();
                    _previewManager.HideCraftingStationPreviw();
                }
                else
                {
                    UpdateNearestCraftingStation();
                }
            }
        }
        ManualCraftingStationInteract FindCraftingStationById(string id)
        {
            foreach (var station in _craftingStationsInRange.Values)
            {
                if (station.craftingStation.CraftingStationId == id)
                {
                    return station;
                }
            }
            return null;
        }
        
        public void ShowPreviewPanel(CraftingStation craftingStation)
        {
            if (PreviewPanelUI != null)
            {
                PreviewPanelUI.SetActive(true);
                _previewManager.ShowCraftingStationPreview(craftingStation);
            }
        }

        public void HidePreviewPanel()
        {
            if (PreviewPanelUI != null)
            {
                PreviewPanelUI.SetActive(false);
                _previewManager.HideCraftingStationPreviw();
            }
        }


        void UpdateNearestCraftingStation()
        {
            if (_craftingStationsInRange.Count == 0)
            {
                HidePreviewPanel();
                return;
            }

            var nearestStation = _craftingStationsInRange.Values
                .OrderBy(station => Vector3.Distance(transform.position, station.transform.position))
                .FirstOrDefault();

            if (nearestStation != null)
            {
                ShowPreviewPanel(nearestStation.craftingStation);
            }
        }

        public void ShowSelectedCraftingStationPreviewPanel(CraftingStation craftingStation)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
            _previewManager.ShowCraftingStationPreview(craftingStation);
        }
        public void HideSelectedCraftingStationPreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
            _previewManager.HideCraftingStationPreviw();
        }
        public bool IsPreviewedCraftingStation(ManualCraftingStationInteract manualCraftingStationInteract)
        {
            if (_isInteractLocked) return false;
            var isPreviewed = CurrentPreviewedStationInteract != null &&
                              CurrentPreviewedStationInteract.UniqueID == manualCraftingStationInteract.UniqueID;

            return isPreviewed;
        }
        public bool TryBeginInteraction(ManualCraftingStationInteract manualCraftingStationInteract)
        {
            if (_isInteracting || Time.time - _lastInteractTime < _interactCooldown) return false;

            if (CurrentPreviewedStationInteract?.UniqueID != manualCraftingStationInteract.UniqueID) return false;

            _isInteracting = true;
            _lastInteractTime = Time.time;

            try
            {
                manualCraftingStationInteract.craftingStation.Interact();
                return true;
            }
            finally
            {
                _isInteracting = false;
            }
        }
    }
}
