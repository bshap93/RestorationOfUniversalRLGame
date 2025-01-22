using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.InventoryEngine;
using TMPro;
using UnityEngine;

public class ChestPanelInstance : MonoBehaviour
{
    public CanvasGroup chestCanvasGroup;
    public TMP_Text ChestIDText;
    public ContainerController containerController;
    public InventoryDisplay chestInventoryDisplay;


    ContainerInventory _chestInventory;

    void Start()
    {
    }
    public void SetInventory(ContainerInventory getInventory)
    {
        if (_chestInventory == null || chestInventoryDisplay == null)
        {
            Debug.LogWarning("Null inventory or display");
            return;
        }

        _chestInventory = getInventory;
    }
}
