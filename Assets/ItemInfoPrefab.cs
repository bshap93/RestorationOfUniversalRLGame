using Gameplay.Player.Inventory;
using Michsky.MUIP;
using Project.Gameplay.Events;
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
    public ManualItemPicker manualItemPicker;

    [FormerlySerializedAs("_item")] public InventoryItem item;
    int _quantity;
    public void SetItem(InventoryItem itemVar, ManualItemPicker manualItemPickerVar)
    {
        manualItemPicker = manualItemPickerVar;
        item = itemVar;
        itemImage.sprite = itemVar.Icon;
        itemName.text = itemVar.ItemName;
        itemType.text = "TBI";
        itemQuantity.text = itemVar.Quantity.ToString();
        _quantity = itemVar.Quantity;

        Debug.Log("ManualItemPicker: " + manualItemPickerVar);
    }


    public void TakeItem()
    {
        if (manualItemPicker != null)
        {
            Debug.Log("Taking item");
            manualItemPicker.PickItem();
        }
    }

    public void ShowItemInfo()
    {
        Debug.Log("Info item");
        ItemEvent.Trigger("ShowItemInfo", item, null);
    }
}
