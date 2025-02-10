using UnityEngine;
using UnityEngine.Serialization;

namespace Project.UI.HUD
{
    public class EnterScenePromptManager : MonoBehaviour
    {
        [FormerlySerializedAs("EnterScenePromptUI")] [FormerlySerializedAs("PickupPromptUI")]
        public GameObject enterScenePromptUI; // Reference to the pickup prompt UI element


        void Start()
        {
            enterScenePromptUI.SetActive(false);
        }

        public void ShowChangeScnenePrompt()
        {
            if (enterScenePromptUI != null) enterScenePromptUI.SetActive(true);
        }

        public void HideChangeScenePrompt()
        {
            enterScenePromptUI.SetActive(false);
        }
    }
}
