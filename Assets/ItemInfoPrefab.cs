using Gameplay.Player.Inventory;
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
    public ManualItemPicker manualItemPicker;

    [FormerlySerializedAs("_item")] public InventoryItem item;
    int _quantity;
    public void SetItem(InventoryItem itemVar, ManualItemPicker manualItemPickerVar)
    {
        manualItemPicker = manualItemPickerVar;
        item = itemVar;

        itemImage.sprite = itemVar.Icon;
        itemName.text = itemVar.ItemName;
        itemType.text = "TBI"; // Set this to the item's type if applicable
        itemQuantity.text = itemVar.Quantity.ToString();
        _quantity = itemVar.Quantity;

        // Dynamically adjust the TakeItem button text
        if (manualItemPicker != null)
        {
            if (manualItemPicker.NotPickable)
                takeItemButton.buttonText = "Use";
            else
                takeItemButton.buttonText = "Take";
        }

    }


    public void TakeItem()
    {
        if (manualItemPicker != null)
        {
            if (manualItemPicker.NotPickable)
            {
                Debug.Log("Using item");
                manualItemPicker.UseItem();
            }
            else
            {
                Debug.Log("Taking item");
                manualItemPicker.PickItem();
            }
        }
    }
}
