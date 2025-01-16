using Michsky.MUIP;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
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

    CookingDepositInventory _cookingDepositInventory;
    CookingQueueInventory _cookingQueueInventory;
    FuelInventory _fuelInventory;

    public void StartCooking()
    {
        _cookingQueueInventory.StartCookingCurrentRecipe();
    }

    public void SetCookingDepositInventory(CookingDepositInventory cookingDepositInventory)
    {
        _cookingDepositInventory = cookingDepositInventory;
        cookingDepositInventoryDisplay.TargetInventoryName = _cookingDepositInventory.name;
    }

    public void SetCookingQueueInventory(CookingQueueInventory cookingQueueInventory)
    {
        _cookingQueueInventory = cookingQueueInventory;
        cookingQueueInventoryDisplay.TargetInventoryName = _cookingQueueInventory.name;
    }

    public void SetFuelInventory(FuelInventory fuelInventory)
    {
        _fuelInventory = fuelInventory;
        _fuelInventory.cookingStationID =
            cookingStationController.CookingStation.CraftingStationId;

        fuelInventoryDisplay.TargetInventoryName = _fuelInventory.name;
    }
}
