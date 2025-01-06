using System.Collections.Generic;
using MoreMountains.Feedbacks;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    [CreateAssetMenu(
        fileName = "Crafting", menuName = "Crafting/CraftingRecipe", order = 1)]
    public class CraftingRecipe : ScriptableObject
    {
        [Tooltip("If false, item can be crafted without a crafting station.")]
        public bool NeedsCraftingStation;

        public CraftingStationType CraftingStationTypeNeeded;

        [Tooltip("If false, item can be crafted without tools.")]
        public bool RequiresTools;
        [Tooltip("Optional. If the recipe requires tools, specify them here.")]
        public List<InventoryTool> RequiredTools;

        [Tooltip("If false, item can be crafted without materials.")]
        public bool RequiresMaterials;


        [Tooltip(
            "If true, the recipe only needs to be set and then after a set amount of time, the item will be crafted.")]
        public bool IsCraftingPassive;


        [Tooltip("The time it takes to craft the item.")]
        public float CraftingTime;

        public MMFeedbacks StartCraftingFeedbacks;
        public MMFeedbacks CraftingCompleteFeedbacks;
    }
}
