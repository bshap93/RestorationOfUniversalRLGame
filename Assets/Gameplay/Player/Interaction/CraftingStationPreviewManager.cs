using System.Collections.Generic;
using System.Linq;
using HighlightPlus;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Prefabs.UI.PrefabRequiredScripts;
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

        public string PreviewPanelTag = "CraftingStationPanel"; // Tag to find panel in scene
        readonly Dictionary<string, ManualCraftingStationInteract> _craftingStationsInRange = new();
        readonly float _interactCooldown = 0.5f;
        TMPCraftingStationDetails _craftingStationDetails;


        ManualCraftingStationInteract _currentPreviewedStationInteract;
        HighlightManager _highlightManager;

        bool _isInteracting;
        bool _isInteractLocked;
        bool _isSorting;
        float _lastInteractTime;
        PreviewManager _previewManager;

        public ManualCraftingStationInteract CurrentPreviewedStationInteract
        {
            get => _currentPreviewedStationInteract;
            private set
            {
                var oldStation = _currentPreviewedStationInteract;
                _currentPreviewedStationInteract = value;

                if (oldStation != value)
                {
                    if (value != null)
                    {
                        ShowPreviewPanel(value.craftingStation);
                        Debug.Log($"Set current previewed station to: {value.UniqueID}");
                    }
                    else
                    {
                        HidePreviewPanel();
                        Debug.Log("Cleared current previewed station");
                    }
                }
            }
        }

        void Awake()
        {
            // Find the preview panel in scene if not set
            if (PreviewPanelUI == null)
            {
                PreviewPanelUI = GameObject.FindWithTag(PreviewPanelTag);
                if (PreviewPanelUI == null)
                    Debug.LogError(
                        "CraftingStationPreviewManager: Could not find preview panel with tag: " + PreviewPanelTag);
            }

            // Get the details component
            if (PreviewPanelUI != null)
            {
                _craftingStationDetails = PreviewPanelUI.GetComponentInChildren<TMPCraftingStationDetails>();
                if (_craftingStationDetails == null)
                    Debug.LogError(
                        "CraftingStationPreviewManager: Could not find TMPCraftingStationDetails in preview panel");
            }
        }

        void Start()
        {
            _previewManager = FindObjectOfType<PreviewManager>();
            _highlightManager = FindObjectOfType<HighlightManager>();

            if (_previewManager == null) Debug.LogWarning("PreviewManager not found in the scene");
            if (_highlightManager == null) Debug.LogWarning("HighlightManager not found in the scene");

            // Initially hide the panel
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
        }

        void Update()
        {
            if (!_isSorting && _craftingStationsInRange.Count > 0) UpdateNearestCraftingStation();
        }

        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == "CraftingStationRangeEntered")
            {
                var craftingStationId = mmEvent.StringParameter;
                var position = mmEvent.Vector3Parameter;

                var station = FindCraftingStationById(craftingStationId);
                if (station != null) HandleCraftingStationEntered(station, position);
            }
            else if (mmEvent.EventName == "CraftingStationRangeExited")
            {
                var craftingStationId = mmEvent.StringParameter;
                var station = FindCraftingStationById(craftingStationId);
                if (station != null) HandleCraftingStationExited(station);
            }
        }

        void HandleCraftingStationEntered(ManualCraftingStationInteract craftingStationInteract, Vector3 position)
        {
            if (!_craftingStationsInRange.ContainsKey(craftingStationInteract.UniqueID))
            {
                Debug.Log($"Adding station to range: {craftingStationInteract.UniqueID}");
                _craftingStationsInRange.Add(craftingStationInteract.UniqueID, craftingStationInteract);
                _highlightManager?.SelectObject(craftingStationInteract.transform);

                // If this is the first/only station, set it as current
                if (_craftingStationsInRange.Count == 1)
                    CurrentPreviewedStationInteract = craftingStationInteract;
                else
                    UpdateNearestCraftingStation();
            }
        }

        void HandleCraftingStationExited(ManualCraftingStationInteract craftingStationInteract)
        {
            if (_craftingStationsInRange.ContainsKey(craftingStationInteract.UniqueID))
            {
                Debug.Log($"Removing station from range: {craftingStationInteract.UniqueID}");
                var wasCurrent = craftingStationInteract == CurrentPreviewedStationInteract;
                _craftingStationsInRange.Remove(craftingStationInteract.UniqueID);
                _highlightManager?.UnselectObject(craftingStationInteract.transform);

                if (wasCurrent)
                {
                    if (_craftingStationsInRange.Count == 0)
                        CurrentPreviewedStationInteract = null;
                    else
                        UpdateNearestCraftingStation();
                }
            }
        }

        void UpdateNearestCraftingStation()
        {
            if (_isInteracting) return;

            _isSorting = true;

            try
            {
                if (_craftingStationsInRange.Count == 0)
                {
                    CurrentPreviewedStationInteract = null;
                    return;
                }

                var nearestStation = _craftingStationsInRange.Values
                    .OrderBy(station => Vector3.Distance(transform.position, station.transform.position))
                    .FirstOrDefault();

                if (nearestStation != CurrentPreviewedStationInteract) CurrentPreviewedStationInteract = nearestStation;
            }
            finally
            {
                _isSorting = false;
            }
        }

        public bool IsPreviewedCraftingStation(ManualCraftingStationInteract manualCraftingStationInteract)
        {
            if (manualCraftingStationInteract == null)
            {
                CurrentPreviewedStationInteract = manualCraftingStationInteract;

                Debug.Log("IsPreviewedCraftingStation check - Checking null station, Result: false");

                if (CurrentPreviewedStationInteract == null)
                {
                    Debug.Log("IsPreviewedCraftingStation check - Current station is null");
                    return false;
                }
            }


            return true;
        }

        public bool TryBeginInteraction(ManualCraftingStationInteract manualCraftingStationInteract)
        {
            if (_isInteracting || Time.time - _lastInteractTime < _interactCooldown) return false;

            if (!IsPreviewedCraftingStation(manualCraftingStationInteract)) return false;

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

        ManualCraftingStationInteract FindCraftingStationById(string id)
        {
            foreach (var station in _craftingStationsInRange.Values)
                if (station.craftingStation.CraftingStationId == id)
                    return station;

            return null;
        }

        // UI Methods for showing/hiding preview panel
        public void ShowPreviewPanel(CraftingStation craftingStation)
        {
            if (PreviewPanelUI != null)
            {
                PreviewPanelUI.SetActive(true);
                if (_craftingStationDetails != null) _craftingStationDetails.DisplayPreview(craftingStation);
                _previewManager?.ShowCraftingStationPreview(craftingStation);
                Debug.Log($"Showing preview panel for station: {craftingStation.CraftingStationName}");
            }
            else
            {
                Debug.LogWarning("ShowPreviewPanel: PreviewPanelUI is null!");
            }
        }

        public void HidePreviewPanel()
        {
            if (PreviewPanelUI != null)
            {
                PreviewPanelUI.SetActive(false);
                if (_craftingStationDetails != null) _craftingStationDetails.Hide();
                _previewManager?.HideCraftingStationPreviw();
                Debug.Log("Hiding preview panel");
            }
        }

        // UI Methods
        public void ShowSelectedCraftingStationPreviewPanel(CraftingStation craftingStation)
        {
            if (PreviewPanelUI != null)
            {
                PreviewPanelUI.SetActive(true);
                _previewManager?.ShowCraftingStationPreview(craftingStation);
                Debug.Log($"Showing selected preview for station: {craftingStation.CraftingStationName}");
            }
        }

        public void HideSelectedCraftingStationPreviewPanel()
        {
            if (PreviewPanelUI != null)
            {
                PreviewPanelUI.SetActive(false);
                _previewManager?.HideCraftingStationPreviw();
                Debug.Log("Hiding selected preview panel");
            }
        }
    }
}
