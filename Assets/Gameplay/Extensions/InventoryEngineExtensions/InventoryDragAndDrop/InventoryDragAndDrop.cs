﻿using System.Collections.Generic;
using Gameplay.ItemManagement.InventoryTypes;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.Extensions.InventoryEngineExtensions.InventoryDragAndDrop
{
    public class InventoryDragAndDrop : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        Canvas _canvas;
        Inventory _inventory;
        InventoryItem _item;
        string _playerID;
        GraphicRaycaster _raycaster;
        List<RaycastResult> _raycastResults;
        InventorySlot _slot;

        void Awake()
        {
            _raycaster = GetComponent<GraphicRaycaster>();
            _canvas = GetComponent<Canvas>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _slot = null;
            Raycast(eventData);
            foreach (var result in _raycastResults)
            {
                _slot = result.gameObject.GetComponent<InventorySlot>();
                if (_slot == null) continue;
                _inventory = _slot.ParentInventoryDisplay.TargetInventory;
                _playerID = _inventory.PlayerID;
                _item = _inventory.Content[_slot.Index];
                if (InventoryItem.IsNull(_item)) _slot = null;
                return;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_slot == null) return;
            var screenPoint = Input.mousePosition;
            _slot.IconImage.transform.SetParent(transform, false);
            if (_canvas.worldCamera != null)
            {
                screenPoint.z = _canvas.planeDistance;
                _slot.IconImage.transform.position = _canvas.worldCamera.ScreenToWorldPoint(screenPoint);
            }
            else
            {
                _slot.IconImage.transform.position = screenPoint;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_slot == null) return;

            // Reset icon position after drag
            _slot.IconImage.transform.SetParent(_slot.transform, false);
            _slot.IconImage.transform.localPosition = Vector3.zero;

            Raycast(eventData);

            foreach (var result in _raycastResults)
            {
                var destinationSlot = result.gameObject.GetComponent<InventorySlot>();
                if (destinationSlot == null) continue;

                var destinationInventory = destinationSlot.ParentInventoryDisplay.TargetInventory;
                var destinationItem = destinationInventory.Content[destinationSlot.Index];
                var isDestinationEmpty = InventoryItem.IsNull(destinationItem);

                // Move within the same inventory
                if (_inventory == destinationInventory &&
                    ((_item.CanMoveObject && isDestinationEmpty) ||
                     (_item.CanSwapObject && !isDestinationEmpty && destinationItem.CanSwapObject)))
                {
                    _inventory.MoveItem(_slot.Index, destinationSlot.Index);
                }
                // Equip the item
                else if (_item.IsEquippable && _item.TargetEquipmentInventoryName == destinationInventory.name)
                {
                    if (isDestinationEmpty)
                    {
                        _item.Equip(_playerID);
                        _inventory.MoveItemToInventory(_slot.Index, destinationInventory, destinationSlot.Index);
                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ItemEquipped,
                            destinationSlot,
                            destinationInventory.name,
                            _item,
                            0,
                            destinationSlot.Index,
                            _playerID
                        );
                    }
                    else
                    {
                        // Unequip the destination item
                        destinationItem.UnEquip(_playerID);
                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ItemUnEquipped,
                            destinationSlot,
                            destinationInventory.name,
                            destinationItem,
                            0,
                            destinationSlot.Index,
                            _playerID
                        );

                        // Equip the new item
                        _item.Equip(_playerID);
                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ItemEquipped,
                            destinationSlot,
                            destinationInventory.name,
                            _item,
                            0,
                            destinationSlot.Index,
                            _playerID
                        );

                        // Swap items between inventories
                        var tempItem = _item.Copy();
                        _inventory.Content[_slot.Index] = destinationItem.Copy();
                        destinationInventory.Content[destinationSlot.Index] = tempItem;

                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ContentChanged,
                            null,
                            destinationInventory.name,
                            null,
                            0,
                            0,
                            _playerID
                        );

                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ContentChanged,
                            null,
                            _inventory.name,
                            null,
                            0,
                            0,
                            _playerID
                        );
                    }
                }
                // Handle moving to MainInventory or other inventories
                else if (_inventory != destinationInventory && isDestinationEmpty)
                {
                    // If moving to MainInventory and item is equippable, trigger unequip
                    if (destinationInventory is MainInventory && _item.Equippable)
                    {
                        _item.UnEquip(_playerID);
                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ItemUnEquipped,
                            _slot,
                            _inventory.name,
                            _item,
                            0,
                            _slot.Index,
                            _playerID
                        );

                        Debug.Log($"Unequipped {_item.ItemID} when moved to MainInventory.");
                    }

                    // If moving to HotbarInventory and item is equippable, trigger unequip
                    if (destinationInventory is HotbarInventory && _item.Equippable)
                    {
                        _item.UnEquip(_playerID);
                        MMInventoryEvent.Trigger(
                            MMInventoryEventType.ItemUnEquipped,
                            _slot,
                            _inventory.name,
                            _item,
                            0,
                            _slot.Index,
                            _playerID
                        );

                        Debug.Log($"Unequipped {_item.ItemID} when moved to HotbarInventory.");
                    }

                    _inventory.MoveItemToInventory(_slot.Index, destinationInventory, destinationSlot.Index);
                    MMInventoryEvent.Trigger(
                        MMInventoryEventType.ContentChanged,
                        null,
                        _inventory.name,
                        null,
                        0,
                        0,
                        _playerID
                    );

                    MMInventoryEvent.Trigger(
                        MMInventoryEventType.ContentChanged,
                        null,
                        destinationInventory.name,
                        null,
                        0,
                        0,
                        _playerID
                    );
                }

                // Stop processing if the item has been successfully moved
                break;
            }

            _raycastResults.Clear();
        }


        void Raycast(PointerEventData eventData)
        {
            _raycastResults = new List<RaycastResult>();
            _raycaster.Raycast(eventData, _raycastResults);
        }
    }
}
