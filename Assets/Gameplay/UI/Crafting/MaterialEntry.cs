using Project.Gameplay.ItemManagement.InventoryTypes.Materials;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MaterialEntry : MonoBehaviour
{
    public TMP_Text materialName;
    public Image materialImage;
    public TMP_Text materialQuantity;

    public void SetMaterial(CraftingMaterial material)
    {
        materialName.text = material.item.ItemName;
        materialImage.sprite = material.item.Icon;
        materialQuantity.text = material.quantity.ToString();
    }
}
