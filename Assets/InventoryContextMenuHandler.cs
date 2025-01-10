using System;
using Michsky.MUIP;
using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryContextMenuHandler : MonoBehaviour
{
    public ContextMenuManager ContextMenu;
    public InventoryDisplay InventoryDisplay; // Reference to the InventoryDisplay
    public string playerID = "Player1"; // The player ID of the inventory

    void Update()
    {
        // Check for a left-click
        if (Input.GetMouseButtonDown(0))
        {
            var clickedObject = EventSystem.current.currentSelectedGameObject;

            if (clickedObject != null && clickedObject.GetComponent<InventorySlot>() != null)
            {
                var slot = clickedObject.GetComponent<InventorySlot>();

                if (slot.CurrentItem != null) ShowContextMenu(slot);
            }
        }

        // Close the context menu on right-click
        if (Input.GetMouseButtonDown(1)) ContextMenu.Close();
    }

    void ShowContextMenu(InventorySlot slot)
    {
        // Set the context menu position
        ContextMenu.SetContextMenuPosition();
        ContextMenu.Open();

        // Populate context menu with options
        AddContextButton("Use", () => slot.CurrentItem.Use(playerID));
        AddContextButton("Equip", () => slot.CurrentItem.Equip(playerID));
        AddContextButton("Drop", () => slot.CurrentItem.Drop(playerID));
    }

    void AddContextButton(string buttonText, Action onClickAction)
    {
        // Create a new button for the context menu
        var button = Instantiate(ContextMenu.contextButton, ContextMenu.contextContent.transform);
        button.GetComponentInChildren<Text>().text = buttonText;
        button.GetComponent<Button>().onClick.AddListener(() => onClickAction.Invoke());
    }
}
