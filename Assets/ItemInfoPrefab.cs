using Michsky.MUIP;
using Project.Gameplay.Interactivity.Items;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ItemInfoPrefab : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemType;
    public Image typeIcon;
    public TMP_Text itemQuantity;
    public ButtonManager takeItemButton;
    public ButtonManager infoItemButton;

    [FormerlySerializedAs("_item")] public InventoryItem item;
    int _quantity;
    public void SetItem(InventoryItem item)
    {
        this.item = item;
        itemImage.sprite = item.Icon;
        itemName.text = item.ItemName;
        itemType.text = "TBI";
        itemQuantity.text = item.Quantity.ToString();
        _quantity = item.Quantity;
    }

    public void TakeItem()
    {
        if (_quantity > 0)
        {
            _quantity--;
            itemQuantity.text = _quantity.ToString();
        }
    }
}
