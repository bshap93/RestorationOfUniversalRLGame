using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MoreMountains.Feedbacks;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Interactivity.CraftingStation
{
    [Serializable]
    public enum CraftingStationType
    {
        CookingStation,
        ForgingStation,
        ItemRemovalStation
    }

    [Serializable]
    public class ActivationResource
    {
        public InventoryItem ActivationItem;
        public int ActivationQuantity;
    }

    [Serializable]
    /// <summary>
    /// Objects that can be interacted with by the player
    /// 
    /// </summary>
    [CreateAssetMenu(
        fileName = "Crafting", menuName = "Crafting/CraftingStation", order = 1)]
    public class CraftingStation : Interactable
    {
        [Tooltip("The ID of the crafting station")]
        public string CraftingStationId;

        [Tooltip("The name of the inventory being used as the source of items to craft")]
        public string SourceInventoryName;

        [Tooltip("The name of the inventory where the crafted item will be placed")]
        public string TargetInventoryName;

        [Tooltip("The name of the inventory where uncrafted materials will be placed")] [SerializeField]
        protected string QueueInventoryName;

        [Tooltip("The name of the inventory finished items")] [SerializeField]
        protected string DepositInventoryName;

        [Tooltip("Fuel inventory")] [SerializeField]
        protected string FuelInventoryName;

        [Tooltip("Is the crafting station active. Or does it need action")]
        public bool IsCraftingStationActive;

        [FormerlySerializedAs("InitialActivationItem")]
        [Tooltip(
            "If a resource is an activation requirement, " +
            "this is the item that will be consumed when the station is activated" +
            "if present in source inventory")]
        public ActivationResource InitialActivationResources;

        [Tooltip("The recipes that can be crafted at this station")]
        public List<CraftingRecipe> CraftingRecipes = new();

        [Tooltip("The multiplier for crafting speed and output volume")] [Range(0, 1)]
        public float CraftingStationEfficiency = 0.5f;

        [Tooltip("The maximum number of items that can be crafted at once")]
        public int ConcurrentCraftingLimit = 1;

        [Tooltip("Where the items will spawn when the crafting station is full")]
        public MMSpawnAroundProperties OverflowSpawnProperties;


        [Tooltip("The type of crafting station")]
        public CraftingStationType StationType;

        [Header("Basic info")] [Tooltip("The name of the crafting station")]
        public string CraftingStationName;
        [Tooltip("Short description of the crafting station")]
        public string ShortDescription;
        [Tooltip("Long description of the crafting station")]
        public string Description;

        [Header("Image")]
        [Tooltip("the icon that will be shown on the preview of the crafting station and in the crafting UI")]
        public Sprite Icon;

        [Header("Feedbacks")] [Tooltip("Feedbacks to play when the crafting station is interacted with")] [CanBeNull]
        public MMFeedbacks InteractFeedbacks;
        [Tooltip("Feedbacks to play when crafting is in progress")] [CanBeNull]
        public MMFeedbacks CraftingInProgressFeedbacks;
        [Tooltip("Feedbacks to play when crafting is complete")] [CanBeNull]
        public MMFeedbacks CraftingCompleteFeedbacks;

        [Tooltip("Feedbacks to play when the depopsitory inventory is full")]
        public MMFeedbacks CraftingStationDepositInventoryFullFeedbacks;

        [Header("Audio")] [Tooltip("The sound to play when the crafting station is interacted with")]
        public AudioClip InteractSound;
        [Tooltip("The sound to play when crafting is in progress")]
        public AudioClip CraftingInProgressSound;
        [Tooltip("The sound to play when crafting is complete")]
        public AudioClip CraftingCompleteSound;
        protected Inventory _depositInventory;
        protected Inventory _queueInventory;

        protected Inventory _sourceInventory;
        protected Inventory _targetInventory;

        public virtual Inventory SourceInventory(string playerID)
        {
            if (SourceInventoryName == null) return null;

            _sourceInventory = Inventory.FindInventory(SourceInventoryName, playerID);
            return _sourceInventory;
        }

        public virtual Inventory TargetInventory(string playerID)
        {
            if (TargetInventoryName == null) return null;

            _targetInventory = Inventory.FindInventory(TargetInventoryName, playerID);
            return _targetInventory;
        }

        public virtual Inventory QueueInventory(string playerID)
        {
            if (QueueInventoryName == null) return null;

            _queueInventory = Inventory.FindInventory(QueueInventoryName, playerID);
            return _queueInventory;
        }

        public virtual Inventory DepositInventory(string playerID)
        {
            if (DepositInventoryName == null) return null;

            _depositInventory = Inventory.FindInventory(DepositInventoryName, playerID);
            return _depositInventory;
        }

        public virtual Inventory FuelInventory(string playerID)
        {
            if (FuelInventoryName == null) return null;

            return Inventory.FindInventory(FuelInventoryName, playerID);
        }

        public virtual void Interact()
        {
            InteractFeedbacks?.PlayFeedbacks();
        }

        public virtual CraftingStation Copy()
        {
            var name = this.name;
            var clone = Instantiate(this);
            clone.name = name;
            return clone;
        }
    }
}
