using System;
using Gameplay.Combat.Tools;
using MoreMountains.InventoryEngine;
using MoreMountains.TopDownEngine;
using Plugins.TopDownEngine.ThirdParty.MoreMountains.InentoryEngine.InventoryEngine.Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat.Tools
{
    [CreateAssetMenu(fileName = "TorchItem", menuName = "Inventory/Equipment/Torch", order = 2)]
    [Serializable]
    public class TorchItem : BaseItem
    {
        [Header("Torch Properties")] [Tooltip("The torch prefab to instantiate when equipped.")]
        public GameObject TorchPrefab;

        [Tooltip("Which inventory this torch should be equipped to.")]
        public string EquipmentInventoryName = "EquipmentInventory";

        [Tooltip("Should this torch auto-equip when picked up.")]
        public bool AutoEquipOnPickup;

        [FormerlySerializedAs("_itemType")] [SerializeField]
        ItemType itemType = ItemType.Torch;

        public override bool Equip(string playerID)
        {
            // Find the character that owns this inventory
            var character = TargetInventory(playerID)?.Owner?.GetComponent<Character>();
            if (character == null) return false;

            // Find torch handler
            var torchHandler = character.FindAbility<CharacterHandleTorch>();
            if (torchHandler == null)
            {
                Debug.LogWarning($"No torch handler found on character {character.name}");
                return false;
            }

            // Equip the torch
            torchHandler.EquipTorch(TorchPrefab);

            // Move item to equipment inventory if needed
            var equipmentInventory = Inventory.FindInventory(EquipmentInventoryName, playerID);
            if (equipmentInventory != null && TargetInventory(playerID) != equipmentInventory)
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.InventoryOpens, null, equipmentInventory.name, this, 1, 0, playerID);

            return true;
        }

        public override bool UnEquip(string playerID)
        {
            // Find the character that owns this inventory
            var character = TargetInventory(playerID)?.Owner?.GetComponent<Character>();
            if (character == null) return false;

            // Find torch handler
            var torchHandler = character.FindAbility<CharacterHandleTorch>();
            if (torchHandler != null)
            {
                torchHandler.EquipTorch(null); // Unequip the current torch
                return true;
            }

            return false;
        }
    }
}
