using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Prefabs.UI.PrefabRequiredScripts;
using UnityEngine;

namespace Project.UI.HUD
{
    public class PreviewManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        public TMPInventoryDetails InventoryDetails;


        public InventoryItem CurrentPreviewedItem { get; set; }
        void OnEnable()
        {
            this.MMEventStartListening();
            Debug.Log("PreviewManager enabled");
        }

        void OnDisable()
        {
            this.MMEventStopListening();
            Debug.Log("PreviewManager disabled");
        }

        public void OnMMEvent(MMInventoryEvent inventoryEvent)
        {
            if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryOpens)
            {
                Debug.Log("Inventory Opens");
                HidePreview();
            }

            if (inventoryEvent.InventoryEventType == MMInventoryEventType.InventoryCloses)
            {
                Debug.Log("Inventory Closes");
                HidePreview();
            }
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
