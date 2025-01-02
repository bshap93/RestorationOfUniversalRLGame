using MoreMountains.Feedbacks;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    public class CraftingStationPreviewTrigger : MonoBehaviour
    {
        public CraftingStation CraftingStation;
        
        [SerializeField] MMFeedbacks _selectionFeedbacks;
        [SerializeField] MMFeedbacks _deselectionFeedbacks; 
        ManualCraftingStationInteract _craftingStationInteract;

    }
}
