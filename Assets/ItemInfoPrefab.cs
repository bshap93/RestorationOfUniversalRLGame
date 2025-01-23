using Michsky.MUIP;
using Project.Gameplay.Interactivity.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPrefab : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemType;
    public Image typeIcon;
    public TMP_Text itemQuantity;
    public ButtonManager takeItemButton;

    InventoryItem _item;
    int _quantity;
}
