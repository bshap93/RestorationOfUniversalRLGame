using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using TMPro;
using UnityEngine;

public class CookingStationController : MonoBehaviour
{
    [Header("Station Setup")] public CookingRecipe currentRecipe; // The recipe to craft
    public CraftingQueueInventory queueInventory; // Uncooked items
    public CraftingDepositInventory depositInventory; // Cooked items
    public FuelInventory fuelInventory; // Firewood
    public Inventory playerInventory; // Reference to the player's inventory

    public CookingStation CookingStation;

    public GameObject CookingStationInventoryPanel; // Parent for the station inventories UI.


    [Header("Fuel & Progress Tracking")] public float fuelBurnRate = 1f; // Time in seconds to burn one unit of fuel


    [Header("UI & Feedbacks")] public GameObject previewPanel; // UI panel for the station preview
    public TextMeshProUGUI previewText;
    public MMFeedbacks interactFeedbacks;
    public MMFeedbacks craftingFeedbacks;
    public MMFeedbacks completionFeedbacks;

    bool _isCrafting;

    bool _isInPlayerRange;


    void Start()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }

    void Update()
    {
        if (_isInPlayerRange && Input.GetKeyDown(KeyCode.F))
        {
            if (!ValidateFuel())
            {
                TransferFuelFromPlayer(fuelInventory.fuelItemAllowed, 1);

                return;
            }

            OpenInventoryDisplays();

            // if (isCrafting)
            //     StopCrafting();
            // else
            //     StartCrafting();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayerRange = true;
            Debug.Log("Player entered cooking station range");

            // Display the inventories when interacting.

            if (!ValidateFuel())
            {
                ShowPreview("No fuel available. Deposit fuel to start cooking.");
                // Prompt the player to deposit fuel
                PromptFuelDeposit();
            }

            ShowPreview("Press F to interact with the Cooking Station");
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayerRange = false;
            HidePreview();
        }
    }

    public Inventory GetQueueInventory()
    {
        return queueInventory;
    }
    public Inventory GetDepositInventory()
    {
        return depositInventory;
    }
    public FuelInventory GetFuelInventory()
    {
        return fuelInventory;
    }

    void OpenInventoryDisplays()
    {
        Debug.Log("Open the player's inventory and the station's inventory");
        MMGameEvent.Trigger("OpenCraftingStationInventoryParts", stringParameter: "CookingStation01");
    }

    public void PromptFuelDeposit()
    {
        // Example: Show UI to deposit fuel
        Debug.Log("Open fuel deposit UI and the player's inventory");
    }

    void ShowPreview(string message)
    {
        if (previewPanel != null)
        {
            previewPanel.SetActive(true);
            if (previewText != null) previewText.text = message;
        }
    }

    void HidePreview()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }


    bool ValidateFuel()
    {
        return fuelInventory.GetQuantity(fuelInventory.fuelItemAllowed.ItemID) > 0;
    }


    public void TransferFuelFromPlayer(InventoryItem fuelItem, int quantity)
    {
        if (playerInventory.GetQuantity(fuelItem.ItemID) >= quantity)
        {
            if (fuelInventory.AddItem(fuelItem, quantity))
            {
                playerInventory.RemoveItemByID(fuelItem.ItemID, quantity);
                ShowPreview($"Added {quantity} fuel to the station.");
            }
            else
            {
                Debug.Log("Fuel inventory is full!");
                ShowPreview("Fuel inventory is full!");
            }
        }
        else
        {
            Debug.Log("Not enough fuel in player inventory.");
            ShowPreview("Not enough fuel in your inventory.");
        }
    }
}
