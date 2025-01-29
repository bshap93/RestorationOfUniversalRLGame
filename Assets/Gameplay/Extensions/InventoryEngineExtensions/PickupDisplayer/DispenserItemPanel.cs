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

        int _capacity; // Store the original capacity

        public void SetItem(BaseItem item, int remaining, int cap, string type)
        {
            itemImage.sprite = item.Icon;
            itemName.text = item.ItemName;
            dispenserType.text = type;

            _capacity = cap; // Store the original capacity value
            UpdateStock(remaining); // Call update method to ensure it remains consistent
        }

        public void UpdateStock(int remaining)
        {
            remainingItem.text = $"{remaining}/{_capacity}"; // Always keep the correct capacity
        }
    }
}
