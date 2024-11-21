using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Project.UI.HUD
{
    public class PickupPromptManager : MonoBehaviour
    {
        public GameObject PickupPromptUI; // Reference to the pickup prompt UI element
        public GameObject PreviewPanelUI;

        void Start()
        {
            PickupPromptUI.SetActive(false);
        }

        public void ShowPickupPrompt()
        {
            if (PickupPromptUI != null) PickupPromptUI.SetActive(true);
        }

        public void HidePickupPrompt()
        {
            if (PickupPromptUI != null) PickupPromptUI.SetActive(false);
        }

        public void HidePreviewPanel()
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(false);
        }

        public void ShowPreviewPanel(InventoryItem item)
        {
            if (PreviewPanelUI != null) PreviewPanelUI.SetActive(true);
        }
    }
}
