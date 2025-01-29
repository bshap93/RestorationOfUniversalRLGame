using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Core.Events;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using Project.Gameplay.SaveLoad.Triggers;
using TMPro;
using UnityEngine;

public class CookingStationController : MonoBehaviour, ISelectableTrigger, MMEventListener<CookingStationEvent>
{
    public CookingQueueInventory queueInventory; // Uncooked items
    public CookingDepositInventory depositInventory; // Cooked items
    public FuelInventory fuelInventory; // Firewood
    public Inventory playerInventory; // Reference to the player's inventory


    [CanBeNull] public FuelMaterial FuelItemAlreadyAdded;

    public CookingStation CookingStation;


    [Header("Fuel & Progress Tracking")] public float fuelBurnRate = 1f; // Time in seconds to burn one unit of fuel


    [Header("UI & Feedbacks")] public GameObject previewPanel; // UI panel for the station preview
    public TextMeshProUGUI previewText;
    public MMFeedbacks interactFeedbacks;
    public MMFeedbacks craftingFeedbacks;
    public MMFeedbacks completionFeedbacks;
    public MMFeedbacks newRecipeSetFeedbacks;
    CookingRecipe _currentRecipe; // The recipe to craft

    bool _isCrafting;

    bool _isInPlayerRange;


    void Start()
    {
        InitializeInventories();
        if (previewPanel != null) previewPanel.SetActive(false);

        if (fuelInventory.GetQuantity("Firewood") > 0)
            FuelItemAlreadyAdded =
                new FuelMaterial(
                    fuelInventory.fuelItemAllowed, fuelInventory.GetQuantity(fuelInventory.fuelItemAllowed.ItemID));
        else
            FuelItemAlreadyAdded = null;


        if (FuelItemAlreadyAdded != null)
            fuelInventory.TreatAddedItem(
                FuelItemAlreadyAdded.FuelItem.Item,
                FuelItemAlreadyAdded.Quantity);
    }

    void OnEnable()
    {
        InitializeInventories();

        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isInPlayerRange = true;

            if (CookingStation.CraftingStationId == null)
            {
                Debug.LogError("CraftingStationId is null");
                return;
            }


            CookingStationEvent.Trigger("CookingStationInRange", CookingStationEventType.CookingStationInRange, this);

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
            CookingStationEvent.Trigger(
                "CookingStationOutOfRange",
                CookingStationEventType.CookingStationOutOfRange,
                this);

            HidePreview();
        }
    }
    public void OnSelectedItem()
    {
        CookingStationEvent.Trigger("CookingStationSelected", CookingStationEventType.CookingStationSelected, this);
    }
    public void OnUnSelectedItem()
    {
        CookingStationEvent.Trigger("CookingStationDeselected", CookingStationEventType.CookingStationDeselected, this);
    }
    public void OnMMEvent(CookingStationEvent mmEvent)
    {
        if (mmEvent.EventType == CookingStationEventType.TryAddFuel)
        {
            Debug.Log("Received TryAddFuel event");
            TryAddFuel();
        }
    }
    public void TryAddFuel()
    {
        if (_isInPlayerRange)
            if (!ValidateFuel())
            {
                // Reinitialize inventories if reference is lost
                if (playerInventory == null) InitializeInventories();

                // Only try to transfer if we have a valid inventory
                if (playerInventory != null)
                    TransferFuelFromPlayer(fuelInventory.fuelItemAllowed, 1);
                else
                    Debug.LogWarning("Cannot transfer fuel - player inventory is null");
            }
    }

    // Add this method to reinitialize inventory references
    void InitializeInventories()
    {
        if (playerInventory == null)
        {
            playerInventory = Inventory.FindInventory("MainPlayerInventory", "Player1");
            if (playerInventory == null) Debug.LogError("Could not find MainPlayerInventory");
        }
    }

    public bool IsPlayerInRange()
    {
        return _isInPlayerRange;
    }

    public CookingQueueInventory GetQueueInventory()
    {
        return queueInventory;
    }
    public CookingDepositInventory GetDepositInventory()
    {
        return depositInventory;
    }
    public FuelInventory GetFuelInventory()
    {
        return fuelInventory;
    }


    public void PromptFuelDeposit()
    {
        // Example: Show UI to deposit fuel
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
        // Add null check at start of method
        if (playerInventory == null)
        {
            Debug.LogWarning("Cannot transfer fuel - player inventory is null");
            ShowPreview("Cannot transfer fuel - inventory error");
            return;
        }

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
    public void SetCurrentRecipe(CookingRecipe currentRecipe)
    {
        _currentRecipe = currentRecipe;
        newRecipeSetFeedbacks?.PlayFeedbacks();
        if (_currentRecipe != null)
            Debug.Log("Current recipe set to: " + currentRecipe.recipeName);
    }
}
