using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.InteractiveEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Gameplay.Interactivity.InteractivityUI
{
    public class CraftingStationDetails : MonoBehaviour, IDetailsDisplay
    {
        [Header("UI Components")] [SerializeField]
        CanvasGroup canvasGroup;
        [SerializeField] TMP_Text stationNameText;
        [SerializeField] TMP_Text descriptionText;
        [SerializeField] Image stationIcon;
        [SerializeField] TMP_Text statusText; // For showing "Ready", "Cooking", etc.

        public CanvasGroup CanvasGroup => canvasGroup;

        public void DisplayDetails(IPreviewable previewable)
        {
            if (previewable is not CraftingStationBehaviour craftingStation)
                return;

            var data = craftingStation.CraftingStationData;
            stationNameText.text = data.CraftingStationName;
            descriptionText.text = data.ShortDescription;
            stationIcon.sprite = data.Icon;
            statusText.text = "Ready"; // We'll update this when we add cooking state
        }

        public void Hide()
        {
            stationNameText.text = "";
            descriptionText.text = "";
            stationIcon.sprite = null;
            statusText.text = "";
        }
    }
}
