using MoreMountains.InventoryEngine;
using Project.UI.Crafting.Cooking;

namespace Gameplay.ItemManagement
{
    public class CustomDepositInventoryDisplay : InventoryDisplay

    {
        CookStationPanelInstance _cookStationPanelInstance;

        protected override void Awake()
        {
            _cookStationPanelInstance = GetComponentInParent<CookStationPanelInstance>();
            TargetInventoryName = _cookStationPanelInstance.cookingStationController.GetDepositInventory().name;
            base.Awake();
        }
    }
}
