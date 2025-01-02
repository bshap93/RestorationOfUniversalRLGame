using System;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.CraftingStation;
using UnityEngine;

namespace Project.Gameplay.Player.Interaction
{
    public class PlayerInteractionPreviewManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public GameObject PreviewPanelUI;
        public MMFeedbacks SelectionFeedbacks;
        public MMFeedbacks DeselectionFeedbacks;

        public void OnMMEvent(MMGameEvent eventType)
        {
            throw new NotImplementedException();
        }
        public void ShowSelectedInteractablePreviewPanel(CraftingStation craftingStation)
        {
            throw new NotImplementedException();
        }
        public void HideSelectedInteractablePreviewPanel()
        {
            throw new System.NotImplementedException();
        }
    }
}
