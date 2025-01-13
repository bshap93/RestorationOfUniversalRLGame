using TMPro;
using UnityEngine;

namespace Project.UI.HUD
{
    public class PromptManager : MonoBehaviour
    {
        // public GameObject PickupPromptUI; // Reference to the pickup prompt UI element
        public GameObject InteractPromptUI;
        string _interactPromptText;
        TMP_Text _interactPromptTextComponent;

        void Start()
        {
            // PickupPromptUI.SetActive(false);
            InteractPromptUI.SetActive(false);
        }

        public void ShowPickupPrompt()
        {
            // if (PickupPromptUI != null) PickupPromptUI.SetActive(true);
        }

        public void HidePickupPrompt()
        {
            // if (PickupPromptUI != null) PickupPromptUI.SetActive(false);
        }


        public void ShowInteractPrompt(string text)
        {
            if (InteractPromptUI != null)
            {
                InteractPromptUI.SetActive(true);
                _interactPromptTextComponent = InteractPromptUI.GetComponentInChildren<TMP_Text>();
                _interactPromptTextComponent.text = text;
            }
        }

        public void HideInteractPrompt()
        {
            if (InteractPromptUI != null) InteractPromptUI.SetActive(false);
        }
    }
}
