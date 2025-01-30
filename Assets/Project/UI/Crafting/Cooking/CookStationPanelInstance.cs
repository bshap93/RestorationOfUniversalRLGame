using System;
using Michsky.MUIP;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using Project.Gameplay.ItemManagement.InventoryTypes.Fuel;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.UI.Crafting.Cooking
{
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

        [FormerlySerializedAs("_cookingDepositInventory")]
        public CookingDepositInventory cookingDepositInventory;
        [FormerlySerializedAs("_cookingQueueInventory")]
        public CookingQueueInventory cookingQueueInventory;
        [FormerlySerializedAs("_fuelInventory")]
        public FuelInventory fuelInventory;
        CookableItemsDropDown _cookableItemsDropDown;

        void Start()
        {
            if (recipeDropDown != null)
            {
                _cookableItemsDropDown = recipeDropDown.GetComponent<CookableItemsDropDown>();
                if (_cookableItemsDropDown != null && cookingStationController != null &&
                    cookingStationController.CookingStation != null)
                    _cookableItemsDropDown.CraftingStationId =
                        cookingStationController.CookingStation.CraftingStationId;
            }

            if (CookStationIDText != null && cookingStationController != null &&
                cookingStationController.CookingStation != null)
                CookStationIDText.text = cookingStationController.CookingStation.CraftingStationId;
        }

        public void StartCooking()
        {
            cookingQueueInventory.StartCookingCurrentRecipe();
        }

        public void SetCookingDepositInventory(CookingDepositInventory cookingDepositInventory)
        {
            if (cookingDepositInventory == null || cookingDepositInventoryDisplay == null)
            {
                Debug.LogWarning("Null deposit inventory or display");
                return;
            }

            this.cookingDepositInventory = cookingDepositInventory;
            cookingDepositInventoryDisplay.TargetInventoryName = this.cookingDepositInventory.name;
            cookingDepositInventoryDisplay.ChangeTargetInventory(this.cookingDepositInventory.name);
        }

        public void SetCookingQueueInventory(CookingQueueInventory cookingQueueInventory)
        {
            if (cookingQueueInventory == null || cookingQueueInventoryDisplay == null)
            {
                Debug.LogWarning("Null queue inventory or display");
                return;
            }

            this.cookingQueueInventory = cookingQueueInventory;
            cookingQueueInventoryDisplay.TargetInventoryName = this.cookingQueueInventory.name;
            cookingQueueInventoryDisplay.ChangeTargetInventory(this.cookingQueueInventory.name);
        }

        public void SetFuelInventory(FuelInventory fuelInventory)
        {
            if (fuelInventory == null || fuelInventoryDisplay == null)
            {
                Debug.LogWarning("Null fuel inventory or display");
                return;
            }

            this.fuelInventory = fuelInventory;

            // Set the cooking station ID for the fuel inventory
            if (cookingStationController != null && cookingStationController.CookingStation != null)
                this.fuelInventory.cookingStationID = cookingStationController.CookingStation.CraftingStationId;

            fuelInventoryDisplay.TargetInventoryName = this.fuelInventory.name;
            fuelInventoryDisplay.ChangeTargetInventory(this.fuelInventory.name);
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
}
