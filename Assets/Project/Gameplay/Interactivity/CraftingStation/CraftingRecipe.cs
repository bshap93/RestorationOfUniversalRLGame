using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using Project.Gameplay.ItemManagement.InventoryItemTypes;
using UnityEngine;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    [Serializable]
    public class CraftingMaterial
    {
        public InventoryItem material; // Optional: You can replace this with an enum or another identifier.
        public int requiredQuantity;
    }

    [Serializable]
    public class CraftingResult
    {
        public InventoryItem item;
        public int quantity;
        public GameObject prefabDrop;
    }

    public class CraftingRecipe : ScriptableObject
    {
        [Tooltip("If false, item can be crafted without a crafting station.")]
        public bool NeedsCraftingStation;
        [Tooltip("Optional. If the recipe requires a crafting station, specify its ID here.")] [CanBeNull]
        public string CraftingStationId;

        [Tooltip("If false, item can be crafted without tools.")]
        public bool RequiresTools;
        [Tooltip("Optional. If the recipe requires tools, specify them here.")]
        public List<InventoryTool> RequiredTools;

        [Tooltip("If false, item can be crafted without materials.")]
        public bool RequiresMaterials;

        [Tooltip("Optional. If the recipe requires materials, specify them here.")]
        public List<CraftingMaterial> RequiredMaterials = new();

        [Tooltip("The items that will be crafted.")]
        public List<CraftingResult> Results = new();

        [Tooltip(
            "If true, the recipe only needs to be set and then after a set amount of time, the item will be crafted.")]
        public bool IsCraftingPassive;


        [Tooltip("The time it takes to craft the item.")]
        public float CraftingTime;

        public MMFeedbacks StartCraftingFeedbacks;
        public MMFeedbacks CraftingCompleteFeedbacks;
    }
}
