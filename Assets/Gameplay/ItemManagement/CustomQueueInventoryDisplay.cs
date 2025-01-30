using MoreMountains.InventoryEngine;
using Project.UI.Crafting.Cooking;

namespace Gameplay.ItemManagement
{
    public class CustomQueueInventoryDisplay : InventoryDisplay

    {
        CookStationPanelInstance _cookStationPanelInstance;

        protected override void Awake()
        {
            _cookStationPanelInstance = GetComponentInParent<CookStationPanelInstance>();
            TargetInventoryName = _cookStationPanelInstance.cookingStationController.GetQueueInventory().name;
            base.Awake();
        }
    }
}
