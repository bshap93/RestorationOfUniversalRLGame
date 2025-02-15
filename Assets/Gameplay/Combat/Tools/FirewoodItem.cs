using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.Combat.Tools
{
    [CreateAssetMenu(fileName = "FirewoodItem", menuName = "Inventory/Equipment/Firewood", order = 2)]
    [Serializable]
    public class FirewoodItem : BaseItem
    {
        public void BuildSmallFire(Vector3 position)
        {
        }
    }
}
