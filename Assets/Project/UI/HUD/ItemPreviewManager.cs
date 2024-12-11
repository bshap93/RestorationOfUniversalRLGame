using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Prefabs.UI.PrefabRequiredScripts;
using UnityEngine;

namespace Project.UI.HUD
{
    public class PreviewManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public TMPInventoryDetails InventoryDetails;
        public InventoryItem CurrentPreviewedItem { get; set; }


        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent eventType)
        {
        }


        public void ShowPreview(InventoryItem item)
        {
            if (InventoryDetails != null)
            {
                InventoryDetails.DisplayPreview(item);

                CurrentPreviewedItem = item;

                // Make sure CanvasGroup is visible
                var canvasGroup = InventoryDetails.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1;
                    canvasGroup.interactable = true;
                    canvasGroup.blocksRaycasts = true;
                }
            }
        }

        public void HidePreview()
        {
            if (InventoryDetails != null)
            {
                var canvasGroup = InventoryDetails.GetComponent<CanvasGroup>();
                CurrentPreviewedItem = null;
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 0;
                    canvasGroup.interactable = false;
                    canvasGroup.blocksRaycasts = false;
                }
            }
        }
    }
}
