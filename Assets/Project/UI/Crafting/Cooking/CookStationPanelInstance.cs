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
    [RequireComponent(typeof(CookingDepositInventory))]
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

        CookingDepositInventory _cookingDepositInventory;
        CookingQueueInventory _cookingQueueInventory;
        FuelInventory _fuelInventory;

        void Start()
        {
            if (cookingStationController != null)
            {
                _cookingDepositInventory = cookingStationController.GetDepositInventory();

                _cookingQueueInventory = cookingStationController.GetQueueInventory();

                _fuelInventory = cookingStationController.GetFuelInventory();
            }

            if (recipeDropDown != null)
            {
                _cookableItemsDropDown = recipeDropDown.GetComponent<CookableItemsDropDown>();
                if (_cookableItemsDropDown != null && cookingStationController != null &&
                    cookingStationController.CookingStation != null)
                    _cookableItemsDropDown.craftingStationId =
                        cookingStationController.CookingStation.CraftingStationId;
            }

            if (CookStationIDText != null && cookingStationController != null &&
                cookingStationController.CookingStation != null)
                CookStationIDText.text = cookingStationController.CookingStation.CraftingStationId;
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

        public void ToggleIsStationBurning()
        {
            _fuelInventory.ToggleIsStationBurning();
        }
    }
}
