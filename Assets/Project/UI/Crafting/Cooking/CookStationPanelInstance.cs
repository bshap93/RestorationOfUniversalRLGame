using System;
using Gameplay.Crafting.Cooking;
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
        [Header("Progress Bars")] public MMProgressBar fuelBurntProgressBar;
        public MMProgressBar cookingProgressBar;

        [Header("Cooking Station Controller")] public CookingStationController cookingStationController;

        [Header("Canvas Groups")] public CanvasGroup cookStationCanvasGroup;

        [Header("MUI Dropdown & Button")] public GameObject recipeDropDown;
        public ButtonManager startCookingButtonManager;

        [Header("Inventory Displays")] public InventoryDisplay cookingDepositInventoryDisplay;
        [FormerlySerializedAs("_cookingQueueInventoryDisplay")]
        public InventoryDisplay cookingQueueInventoryDisplay;
        [FormerlySerializedAs("_fuelInventoryDisplay")]
        public InventoryDisplay fuelInventoryDisplay;
        [Header("CookStation ID Text")] public TMP_Text CookStationIDText;
        CookableItemsDropDown _cookableItemsDropDown;

        CookingDepositInventory cookingDepositInventory;
        CookingQueueInventory cookingQueueInventory;
        FuelInventory fuelInventory;

        void Start()
        {
            if (cookingStationController != null)
            {
                cookingDepositInventory = cookingStationController.GetDepositInventory();

                cookingQueueInventory = cookingStationController.GetQueueInventory();

                fuelInventory = cookingStationController.GetFuelInventory();
            }

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

        // Encapsulation Helper Method
        public void HideCookingUI()
        {
            if (cookingStationController != null)
            {
                Debug.Log("Hiding Cooking UI");
                cookingStationController.HideCookingUI();
            }
            else
            {
                Debug.LogError("Cannot hide cooking UI - controller is null");
            }
        }
    }
}
