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
        _cookableItemsDropDown = recipeDropDown.GetComponent<CookableItemsDropDown>();
        _cookableItemsDropDown.CraftingStationId = cookingStationController.CookingStation.CraftingStationId;
        CookStationIDText.text = cookingStationController.CookingStation.CraftingStationId;
    }

    public void StartCooking()
    {
        _cookingQueueInventory.StartCookingCurrentRecipe();
    }

    public void SetCookingDepositInventory(CookingDepositInventory cookingDepositInventory)
    {
        _cookingDepositInventory = cookingDepositInventory;
        cookingDepositInventoryDisplay.TargetInventoryName = _cookingDepositInventory.name;
        cookingDepositInventoryDisplay.ChangeTargetInventory(_cookingDepositInventory.name);
    }

    public void SetCookingQueueInventory(CookingQueueInventory cookingQueueInventory)
    {
        _cookingQueueInventory = cookingQueueInventory;
        cookingQueueInventoryDisplay.TargetInventoryName = _cookingQueueInventory.name;
        cookingQueueInventoryDisplay.ChangeTargetInventory(_cookingQueueInventory.name);
    }

    public void SetFuelInventory(FuelInventory fuelInventory)
    {
        _fuelInventory = fuelInventory;
        _fuelInventory.cookingStationID =
            cookingStationController.CookingStation.CraftingStationId;

        fuelInventoryDisplay.TargetInventoryName = _fuelInventory.name;
        fuelInventoryDisplay.ChangeTargetInventory(_fuelInventory.name);
    }

    public void SetCookStationIDText(string text)
    {
        CookStationIDText.text = text;
    }
}
