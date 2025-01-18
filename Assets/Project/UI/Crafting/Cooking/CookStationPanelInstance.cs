using System;
using Michsky.MUIP;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CookStationPanelInstance : MonoBehaviour
{
    public MMProgressBar fuelBurntProgressBar;
    public MMProgressBar cookingProgressBar;

    public CanvasGroup cookStationCanvasGroup;

    public GameObject recipeDropDown;

    public ButtonManager startCookingButtonManager;
    public CookingStationController cookingStationController;

    [FormerlySerializedAs("_cookingDepositInventoryDisplay")]
    public InventoryDisplay cookingDepositInventoryDisplay;
    [FormerlySerializedAs("_cookingQueueInventoryDisplay")]
    public InventoryDisplay cookingQueueInventoryDisplay;
    [FormerlySerializedAs("_fuelInventoryDisplay")]
    public InventoryDisplay fuelInventoryDisplay;
    public TMP_Text CookStationIDText;
    CookableItemsDropDown _cookableItemsDropDown;

    CookingDepositInventory _cookingDepositInventory;
    CookingQueueInventory _cookingQueueInventory;
    FuelInventory _fuelInventory;

    void Start()
    {
        if (recipeDropDown != null)
        {
            _cookableItemsDropDown = recipeDropDown.GetComponent<CookableItemsDropDown>();
            if (_cookableItemsDropDown != null && cookingStationController != null &&
                cookingStationController.CookingStation != null)
                _cookableItemsDropDown.CraftingStationId = cookingStationController.CookingStation.CraftingStationId;
        }

        if (CookStationIDText != null && cookingStationController != null &&
            cookingStationController.CookingStation != null)
            CookStationIDText.text = cookingStationController.CookingStation.CraftingStationId;
    }

    public void StartCooking()
    {
        _cookingQueueInventory.StartCookingCurrentRecipe();
    }

    public void SetCookingDepositInventory(CookingDepositInventory cookingDepositInventory)
    {
        if (cookingDepositInventory == null || cookingDepositInventoryDisplay == null)
        {
            Debug.LogWarning("Null deposit inventory or display");
            return;
        }

        _cookingDepositInventory = cookingDepositInventory;
        cookingDepositInventoryDisplay.TargetInventoryName = _cookingDepositInventory.name;
        cookingDepositInventoryDisplay.ChangeTargetInventory(_cookingDepositInventory.name);
    }

    public void SetCookingQueueInventory(CookingQueueInventory cookingQueueInventory)
    {
        if (cookingQueueInventory == null || cookingQueueInventoryDisplay == null)
        {
            Debug.LogWarning("Null queue inventory or display");
            return;
        }

        _cookingQueueInventory = cookingQueueInventory;
        cookingQueueInventoryDisplay.TargetInventoryName = _cookingQueueInventory.name;
        cookingQueueInventoryDisplay.ChangeTargetInventory(_cookingQueueInventory.name);
    }

    public void SetFuelInventory(FuelInventory fuelInventory)
    {
        if (fuelInventory == null || fuelInventoryDisplay == null)
        {
            Debug.LogWarning("Null fuel inventory or display");
            return;
        }

        _fuelInventory = fuelInventory;

        if (cookingStationController != null && cookingStationController.CookingStation != null)
            _fuelInventory.cookingStationID = cookingStationController.CookingStation.CraftingStationId;

        fuelInventoryDisplay.TargetInventoryName = _fuelInventory.name;
        fuelInventoryDisplay.ChangeTargetInventory(_fuelInventory.name);
    }

    public void SetCookStationIDText(string text)
    {
        CookStationIDText.text = text;
    }
    public void SetPlayerInventory(Inventory playerInventory)
    {
        throw new NotImplementedException();
    }
}
