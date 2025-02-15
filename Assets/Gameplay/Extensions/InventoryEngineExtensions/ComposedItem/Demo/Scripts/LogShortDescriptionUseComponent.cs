using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.ComposedItem.Demo.Scripts
{
    public class LogShortDescriptionUseComponent : InventoryItem, IOverridable
    {
        public IOverride NewOverride()
        {
            return new Override(this);
        }
        public override bool Use(string playerID)
        {
            Debug.Log(ShortDescription);
            return true;
        }

        class Override : IOverride
        {
            public readonly string Log;
            public Override(InventoryItem item)
            {
                Log = item.ShortDescription;
            }
            public IOverridable Apply(IOverridable overridable)
            {
                ((InventoryItem)overridable).ShortDescription = Log;
                return overridable;
            }
        }
    }
}
