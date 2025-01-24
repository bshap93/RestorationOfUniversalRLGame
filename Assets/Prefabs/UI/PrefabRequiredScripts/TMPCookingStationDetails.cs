using Michsky.MUIP;
using Project.Core.Events;
using Project.Gameplay.ItemManagement.InventoryTypes.Cooking;
using UnityEngine;

namespace Prefabs.UI.PrefabRequiredScripts
{
    public class TMPCookingStationDetails : TMPCraftingStationDetails
    {
        public ButtonManager AddFuelButton;
        public ButtonManager CookingPanelOpenButton;
        CookingStation _cookingStation;

        bool _fuelAdded;

        /// <summary>
        ///     Display the details of a given crafting station.
        /// </summary>
        public virtual void DisplayPreview(CookingStation cookingStationVar)
        {
            Debug.Log("Displaying cooking station preview");
            if (cookingStationVar == null)
            {
                Debug.LogWarning("No cooking station selected. Filling with defaults.");
                FillWithDefaults();
                return;
            }

            _cookingStation = cookingStationVar;
            Debug.Log("Cooking station set: " + cookingStationVar.CraftingStationName);

            if (TMPTitle != null) TMPTitle.text = cookingStationVar.CraftingStationName;
            if (TMPShortDescription != null) TMPShortDescription.text = cookingStationVar.ShortDescription;
            if (TMPDescription != null) TMPDescription.text = cookingStationVar.Description;
            if (TMPEfficiency != null)
                TMPEfficiency.text = cookingStationVar.CraftingStationEfficiency.ToString("P0");

            if (TMPConcurrentCraftingLimit != null)
                TMPConcurrentCraftingLimit.text = cookingStationVar.ConcurrentCraftingLimit.ToString();

            if (TMPIsActive != null)
                TMPIsActive.text = cookingStationVar.IsCraftingStationActive ? "Active" : "Inactive";

            if (Icon != null) Icon.sprite = cookingStationVar.Icon;

            Show();
        }

        public void TryAddFuel()
        {
            if (_cookingStation == null)
            {
                Debug.LogError("No cooking station selected");
                return;
            }

            Debug.Log("Trying to add fuel to cooking station");


            CookingStationEvent.Trigger(
                "TryAddFuel",
                CookingStationEventType.TryAddFuel, null, _cookingStation);
        }

        public void EnableSelectForThisStation()
        {
            CookingStationEvent.Trigger(
                "CookingStationSelected",
                CookingStationEventType.CookingStationSelected,
                null, _cookingStation);
        }
    }
}
