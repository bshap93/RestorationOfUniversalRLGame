using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
{
    public class CustomInventoryInputManager : InventoryInputManager
    {
        protected override void Start()
        {
            base.Start();
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1;
            _canvasGroup = TargetInventoryContainer.GetComponent<CanvasGroup>();
            
            CloseInventory();
        }
        public override void OpenInventory()
        {
            if (CloseList.Count > 0)
                foreach (var playerID in CloseList)
                    MMInventoryEvent.Trigger(
                        MMInventoryEventType.InventoryCloseRequest, null, "", null, 0, 0, playerID);

            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = true;
                _canvasGroup.interactable = true;
                _canvasGroup.alpha = 1;
            }

            // we open our inventory
            MMInventoryEvent.Trigger(
                MMInventoryEventType.InventoryOpens, null, TargetInventoryDisplay.TargetInventoryName,
                TargetInventoryDisplay.TargetInventory.Content[0], 0, 0, TargetInventoryDisplay.PlayerID);

            MMGameEvent.Trigger("inventoryOpens");
            InventoryIsOpen = true;

            StartCoroutine(MMFade.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 1f));
            TargetInventoryContainer.interactable = true;
            TargetInventoryContainer.blocksRaycasts = true;
            StartCoroutine(MMFade.FadeCanvasGroup(Overlay, 0.2f, OverlayActiveOpacity));
        }

        public override void CloseInventory()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.blocksRaycasts = false;
                _canvasGroup.interactable = false;
                _canvasGroup.alpha = 0;
            }

            // we close our inventory
            MMInventoryEvent.Trigger(
                MMInventoryEventType.InventoryCloses, null, TargetInventoryDisplay.TargetInventoryName, null, 0, 0,
                TargetInventoryDisplay.PlayerID);

            MMGameEvent.Trigger("inventoryCloses");
            InventoryIsOpen = false;

            StartCoroutine(MMFade.FadeCanvasGroup(TargetInventoryContainer, 0.2f, 0f));
            TargetInventoryContainer.interactable = false;
            TargetInventoryContainer.blocksRaycasts = false;
            StartCoroutine(MMFade.FadeCanvasGroup(Overlay, 0.2f, OverlayInactiveOpacity));
        }
    }
}
