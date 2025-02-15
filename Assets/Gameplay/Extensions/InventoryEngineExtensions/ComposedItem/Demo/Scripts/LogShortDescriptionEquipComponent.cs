using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.ComposedItem.Demo.Scripts
{
    public class LogShortDescriptionEquipComponent : InventoryItem
    {
        public override bool Equip(string playerID)
        {
            Debug.Log("+" + ShortDescription);
            return true;
        }
        public override bool UnEquip(string playerID)
        {
            Debug.Log("-" + ShortDescription);
            return true;
        }
    }
}
