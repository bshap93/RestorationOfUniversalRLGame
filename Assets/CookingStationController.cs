using System;
using System.Collections;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.CraftingStation;
using Project.Gameplay.Interactivity.Items;
using Project.Gameplay.ItemManagement.InventoryTypes;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[Serializable]
public class CraftingMaterial
{
    [ShowInInspector] public InventoryItem Item; // The base inventory item
    [ShowInInspector] public int Quantity; // How many units are required
    [ShowInInspector] public GameObject prefabDrop; // Optional: The prefab to drop if the inventory is full

    public CraftingMaterial(InventoryItem item, int quantity, float craftingTime = 1f)
    {
        Item = item;
        Quantity = quantity;
    }
}


public class CookingStationController : MonoBehaviour
{
    [Header("Station Setup")] public CraftingRecipe currentRecipe; // The recipe to craft
    public CraftingQueueInventory queueInventory; // Uncooked items
    public CraftingDepositInventory depositInventory; // Cooked items
    public FuelInventory fuelInventory; // Firewood
    public Inventory playerInventory; // Reference to the player's inventory

    public GameObject CookingStationInventoryPanel; // Parent for the station inventories UI.


    [Header("Fuel & Progress Tracking")] public float fuelBurnRate = 1f; // Time in seconds to burn one unit of fuel
    public float craftingProgress; // 0.0 to 1.0
    public float remainingCraftingTime;


    [Header("UI & Feedbacks")] public GameObject previewPanel; // UI panel for the station preview
    public TextMeshProUGUI previewText;
    public MMFeedbacks interactFeedbacks;
    public MMFeedbacks craftingFeedbacks;
    public MMFeedbacks completionFeedbacks;

    Coroutine craftingCoroutine;
    bool isCrafting;

    bool isInPlayerRange;


    void Start()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }

    void Update()
    {
        if (isInPlayerRange && Input.GetKeyDown(KeyCode.F))
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
            isInPlayerRange = true;
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
            isInPlayerRange = false;
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
            Debug.Log("Showing preview panel");
            if (previewText != null) previewText.text = message;
        }
    }

    void HidePreview()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }

    public void DepositFuel(InventoryItem fuelItem, int quantity)
    {
        if (fuelItem == fuelInventory.fuelItemAllowed)
        {
            if (fuelInventory.AddItem(fuelItem, quantity))
            {
                Debug.Log($"Deposited {quantity} of {fuelItem.ItemName} into the fuel inventory.");
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
            Debug.Log($"{fuelItem.ItemName} is not valid fuel for this station.");
            ShowPreview($"{fuelItem.ItemName} cannot be used as fuel.");
        }
    }


    void StartCrafting()
    {
        if (isCrafting || currentRecipe == null) return;

        if (!ValidateFuel())
        {
            ShowPreview("Add fuel to start cooking!");
            // Optionally, prompt the player to deposit fuel.
            return;
        }

        if (!ValidateMaterials())
        {
            ShowPreview("Missing required materials!");
            return;
        }


        isCrafting = true;
        interactFeedbacks?.PlayFeedbacks();

        if (currentRecipe.IsCraftingPassive)
            StartCoroutine(PassiveCraftingProcess());
        else
            ProcessActiveCrafting();
    }

    void StopCrafting()
    {
        isCrafting = false;
        craftingFeedbacks?.StopFeedbacks();
        ShowPreview("Cooking stopped.");
    }

    bool ValidateMaterials()
    {
        foreach (var material in currentRecipe.RequiredMaterials)
            if (queueInventory.GetQuantity(material.Item.ItemID) < material.Quantity)
                return false;

        return true;
    }

    bool ValidateFuel()
    {
        return fuelInventory.GetQuantity(fuelInventory.fuelItemAllowed.ItemID) > 0;
    }


    IEnumerator PassiveCraftingProcess()
    {
        ShowPreview("Cooking in progress...");
        craftingFeedbacks?.PlayFeedbacks();

        yield return new WaitForSeconds(currentRecipe.CraftingTime);

        ConsumeMaterials();
        ProduceResults();

        isCrafting = false;
        craftingFeedbacks?.StopFeedbacks();
        completionFeedbacks?.PlayFeedbacks();
        ShowPreview("Cooking complete!");
    }

    void ProcessActiveCrafting()
    {
        craftingFeedbacks?.PlayFeedbacks();

        ConsumeMaterials();
        ProduceResults();

        isCrafting = false;
        craftingFeedbacks?.StopFeedbacks();
        completionFeedbacks?.PlayFeedbacks();
        ShowPreview("Cooking complete!");
    }

    void ConsumeMaterials()
    {
        foreach (var material in currentRecipe.RequiredMaterials)
            queueInventory.RemoveItemByID(material.Item.ItemID, material.Quantity);
    }

    void ProduceResults()
    {
        foreach (var result in currentRecipe.Results)
            if (!depositInventory.AddItem(result.Item, result.Quantity))
            {
                Debug.Log("Deposit inventory is full!");
                // Optional: Drop the result item as a prefab
                if (result.prefabDrop != null) Instantiate(result.prefabDrop, transform.position, Quaternion.identity);
            }
    }

    public void TransferFuelFromPlayer(InventoryItem fuelItem, int quantity)
    {
        if (playerInventory.GetQuantity(fuelItem.ItemID) >= quantity)
        {
            if (fuelInventory.AddItem(fuelItem, quantity))
            {
                playerInventory.RemoveItemByID(fuelItem.ItemID, quantity);
                Debug.Log($"Transferred {quantity} of {fuelItem.ItemName} to the station.");
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
