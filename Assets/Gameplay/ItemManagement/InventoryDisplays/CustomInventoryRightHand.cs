using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.ItemManagement.InventoryDisplays
{
    public class CustomInventoryRightHand : InventoryDisplay
    {
        public InventorySlot RightHandSlot;

        protected override void OnEnable()
        {
            base.OnEnable();
            InitializeRightHandSlot();
        }

        protected virtual void InitializeRightHandSlot()
        {
            if (RightHandSlot == null)
            {
                Debug.LogWarning("CustomInventoryRightHand: RightHandSlot is not initialized.");
                return;
            }

            RightHandSlot = SlotContainer[0];

            if (RightHandSlot == null) Debug.LogWarning("CustomInventoryRightHand: RightHandSlot is not initialized.");
        }

        /// <summary>
        ///     Catches MMInventoryEvents and acts on them
        /// </summary>
        /// <param name="mmEvent">Inventory event.</param>
        public override void OnMMEvent(MMInventoryEvent mmEvent)
        {
            Debug.Log("mmEvent: " + mmEvent.InventoryEventType);
            // if this event doesn't concern our inventory display, we do nothing and exit
            if (mmEvent.TargetInventoryName != TargetInventoryName) return;

            if (mmEvent.PlayerID != PlayerID) return;

            switch (mmEvent.InventoryEventType)
            {
                case MMInventoryEventType.Select:
                    SetCurrentlySelectedSlot(mmEvent.Slot);
                    break;

                case MMInventoryEventType.Click:
                    ReturnInventoryFocus();
                    SetCurrentlySelectedSlot(mmEvent.Slot);
                    break;

                case MMInventoryEventType.Move:
                    ReturnInventoryFocus();
                    UpdateSlot(mmEvent.Index);

                    break;

                case MMInventoryEventType.ItemUsed:
                    ReturnInventoryFocus();
                    break;

                case MMInventoryEventType.EquipRequest:
                    if (TargetInventory.InventoryType == Inventory.InventoryTypes.Equipment)
                    {
                        // if there's no target inventory set we do nothing and exit
                        if (TargetChoiceInventory == null)
                        {
                            Debug.LogWarning(
                                "InventoryEngine Warning : " + this + " has no choice inventory associated to it.");

                            return;
                        }

                        // we disable all the slots that don't match the right type
                        TargetChoiceInventory.DisableAllBut(ItemClass);
                        // we set the focus on the target inventory
                        TargetChoiceInventory.Focus();
                        TargetChoiceInventory.InEquipSelection = true;
                        // we set the return focus inventory
                        TargetChoiceInventory.SetReturnInventory(this);
                    }

                    break;

                case MMInventoryEventType.ItemEquipped:
                    ReturnInventoryFocus();
                    break;

                case MMInventoryEventType.Drop:
                    ReturnInventoryFocus();
                    break;

                case MMInventoryEventType.ItemUnEquipped:
                    Debug.Log("ItemUnEquipped");
                    RightHandSlot.UnEquip();
                    ReturnInventoryFocus();
                    break;

                case MMInventoryEventType.InventoryOpens:
                    Focus();
                    CurrentlyBeingMovedItemIndex = -1;
                    IsOpen = true;
                    EventSystem.current.sendNavigationEvents = true;
                    break;

                case MMInventoryEventType.InventoryCloses:
                    CurrentlyBeingMovedItemIndex = -1;
                    EventSystem.current.sendNavigationEvents = false;
                    IsOpen = false;
                    SetCurrentlySelectedSlot(mmEvent.Slot);
                    break;

                case MMInventoryEventType.ContentChanged:
                    ContentHasChanged();
                    break;

                case MMInventoryEventType.Redraw:
                    RedrawInventoryDisplay();
                    break;

                case MMInventoryEventType.InventoryLoaded:
                    RedrawInventoryDisplay();
                    if (GetFocusOnStart) Focus();
                    break;
            }
        }
    }
}
