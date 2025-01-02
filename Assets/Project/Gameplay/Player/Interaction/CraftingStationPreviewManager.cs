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

        public ManualCraftingStationInteract CurrentPreviewedStationInteract;
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
            // Skip updates if no crafting stations are in range or if sorting is in progress
            if (_isSorting || _craftingStationsInRange.Count == 0) return;

            UpdateNearestCraftingStation();
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
                var craftingStationId = eventType.StringParameter;
                var position = eventType.Vector3Parameter;

                // Retrieve the CraftingStation instance (You need a mapping mechanism)
                CurrentPreviewedStationInteract = FindCraftingStationById(craftingStationId);
                if (CurrentPreviewedStationInteract != null)
                    HandleCraftingStationEntered(CurrentPreviewedStationInteract, position);
            }
            else if (eventType.EventName == "CraftingStationRangeExited")
            {
                var craftingStationId = eventType.StringParameter;

                CurrentPreviewedStationInteract = FindCraftingStationById(craftingStationId);
                if (CurrentPreviewedStationInteract != null)
                    HandleCraftingStationExited(CurrentPreviewedStationInteract);
            }
        }

        void HandleCraftingStationEntered(ManualCraftingStationInteract craftingStationInteract, Vector3 position)
        {
            if (!_craftingStationsInRange.ContainsKey(craftingStationInteract.UniqueID))
            {
                _craftingStationsInRange.Add(craftingStationInteract.UniqueID, craftingStationInteract);
                _highlightManager.SelectObject(craftingStationInteract.transform);

                // If this is the first station, set it as the previewed one
                if (_craftingStationsInRange.Count == 1)
                {
                    CurrentPreviewedStationInteract = craftingStationInteract;
                    ShowPreviewPanel(craftingStationInteract.craftingStation);
                }

                UpdateNearestCraftingStation();
            }
        }


        void HandleCraftingStationExited(ManualCraftingStationInteract craftingStationInteract)
        {
            if (_craftingStationsInRange.ContainsKey(craftingStationInteract.UniqueID))
            {
                var wasCurrent = craftingStationInteract == CurrentPreviewedStationInteract;
                _craftingStationsInRange.Remove(craftingStationInteract.UniqueID);
                _highlightManager.UnselectObject(craftingStationInteract.transform);

                if (wasCurrent) CurrentPreviewedStationInteract = null; // Clear the previewed station

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
                if (station.craftingStation.CraftingStationId == id)
                    return station;

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
            _isSorting = true;

            try
            {
                Debug.Log($"Updating nearest station. Stations in range: {_craftingStationsInRange.Count}");

                if (_craftingStationsInRange.Count == 0)
                {
                    if (CurrentPreviewedStationInteract != null)
                    {
                        Debug.Log("No stations in range. Clearing CurrentPreviewedStationInteract.");
                        CurrentPreviewedStationInteract = null;
                        HidePreviewPanel();
                    }

                    return;
                }

                var nearestStation = _craftingStationsInRange.Values
                    .OrderBy(station => Vector3.Distance(transform.position, station.transform.position))
                    .FirstOrDefault();

                if (nearestStation != CurrentPreviewedStationInteract)
                {
                    CurrentPreviewedStationInteract = nearestStation;
                    if (nearestStation != null)
                    {
                        Debug.Log($"Nearest station set to: {nearestStation.UniqueID}");
                        ShowPreviewPanel(nearestStation.craftingStation);
                    }
                }
            }
            finally
            {
                _isSorting = false;
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
            Debug.Log("CurrentPreviewedStationInteract: " + CurrentPreviewedStationInteract);

            var isPreviewed = CurrentPreviewedStationInteract != null &&
                              CurrentPreviewedStationInteract.UniqueID == manualCraftingStationInteract.UniqueID;

            Debug.Log("IsPreviewedCraftingStation: " + isPreviewed);

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
