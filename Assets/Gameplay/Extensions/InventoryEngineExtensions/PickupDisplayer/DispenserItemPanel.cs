using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.Extensions.InventoryEngineExtensions.PickupDisplayer
{
    public class DispenserItemPanel : MonoBehaviour
    {
        public Image itemImage;
        public TMP_Text itemName;
        public TMP_Text remainingItem;
        public TMP_Text dispenserType;

        public void SetItem(BaseItem item, int remaining, int cap, string type)
        {
            itemImage.sprite = item.Icon;
            itemName.text = item.ItemName;
            remainingItem.text = $"{remaining}/{cap}";
            dispenserType.text = type;
        }

        // **New method to update stock without resetting everything**
        public void UpdateStock(int remaining)
        {
            remainingItem.text = $"{remaining}/?";
        }
    }
}
