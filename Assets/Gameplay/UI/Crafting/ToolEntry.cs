using Gameplay.ItemManagement.InventoryItemTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay.UI.Crafting
{
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
}
