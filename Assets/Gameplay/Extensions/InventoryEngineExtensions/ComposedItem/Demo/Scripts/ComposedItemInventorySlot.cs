using System.Linq;
using MoreMountains.InventoryEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.ComposedItem.Demo.Scripts
{
    public class ComposedItemInventorySlot : InventorySlot
    {
        public DurabilityBar DurabilityBar;
        public override void DrawIcon(InventoryItem item, int index)
        {
            base.DrawIcon(item, index);
            if (item is not ComposedItem.Scripts.ComposedItem composedItem ||
                !composedItem.Components.Any(component => component is DurabilityUseComponent))
            {
                DurabilityBar.gameObject.SetActive(false);
                return;
            }

            DurabilityBar.Component =
                (DurabilityUseComponent)composedItem.Components.First(component => component is DurabilityUseComponent);

            DurabilityBar.gameObject.SetActive(true);
        }
    }
}
