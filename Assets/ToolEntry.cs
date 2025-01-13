using Project.Gameplay.ItemManagement.InventoryItemTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolEntry : MonoBehaviour
{
    public TMP_Text toolName;
    public Image toolImage;

    public void SetTool(InventoryTool tool)
    {
        toolName.text = tool.ItemName;
        toolImage.sprite = tool.Icon;
    }
}
