// Example implementation of CraftingStationDetails

using Project.Gameplay.Interactivity.InteractiveEntities;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class CraftingStationDetails : MonoBehaviour, IDetailsDisplay
    {
        [SerializeField] private CanvasGroup canvasGroup;
        // Add UI elements for crafting station details

        public CanvasGroup CanvasGroup => canvasGroup;

        public void DisplayDetails(IPreviewable previewable)
        {
            if (previewable is CraftingStationBehaviour craftingStation)
            {
                // Update UI elements with crafting station data
                // e.g., show name, recipes, status, etc.
            }
        }

        public void Hide()
        {
            // Clear/reset UI elements
        }
    }
}
