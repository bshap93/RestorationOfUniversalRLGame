using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.InventoryDisplays
{
    /// <summary>
    ///     Custom implementation of the InventoryHotbar that allows item switching using the mouse wheel and number keys.
    /// </summary>
    public class CustomInventoryHotbar : InventoryDisplay
    {
        [Header("Hotbar")]
        [MMInformation(
            "Here you can define the keys your hotbar will listen to to activate the hotbar's action.",
            MMInformationAttribute.InformationType.Info, false)]
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
		public InputActionProperty HotbarInputAction = new InputActionProperty(
			new InputAction(
				name: "IM_Demo_LeftKey",
				type: InputActionType.Button, 
				binding: "", 
				interactions: "Press(behavior=2)"));
#else
        /// the key associated to the hotbar, that will trigger the action when pressed
        public string[] HotbarKeys = { "1", "2", "3", "4" };
        /// the alt key associated to the hotbar
        public string[] HotbarAltKeys = { "h", "j", "k", "l" };
#endif
        public InventoryInputManager InventoryInputManager;
        public InventorySlot[] InventorySlots = new InventorySlot[4];

        protected override void OnEnable()
        {
            base.OnEnable();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
    HotbarInputAction.action.Enable();
#endif

            // Ensure InventorySlots is populated after a scene change
            InitializeHotbarSlots();
        }

        /// <summary>
        ///     Populates the InventorySlots array from the InventoryDisplay's SlotContainer
        /// </summary>
        protected virtual void InitializeHotbarSlots()
        {
            if (SlotContainer == null || SlotContainer.Count == 0)
            {
                Debug.LogWarning("CustomInventoryHotbar: SlotContainer is empty or not initialized.");
                return;
            }

            // Ensure InventorySlots array is the correct size
            InventorySlots = new InventorySlot[SlotContainer.Count];

            for (var i = 0; i < SlotContainer.Count; i++)
            {
                InventorySlots[i] = SlotContainer[i];
                if (InventorySlots[i] == null) Debug.LogError($"CustomInventoryHotbar: Slot at index {i} is null.");
            }
        }


        /// <summary>
        ///     Executed when the key or alt key gets pressed, triggers the specified action
        /// </summary>
        public virtual void Action(int index)
        {
            if (!InventoryItem.IsNull(TargetInventory.Content[index]))
            {
                var item = TargetInventory.Content[index];
                Debug.Log($"Hotbar slot {index} selected: {item.ItemID}");

                // Unequip current weapon if switching
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.ItemUnEquipped,
                    null,
                    TargetInventory.name,
                    item,
                    item.Quantity,
                    index,
                    PlayerID
                );

                // Equip new weapon
                MMInventoryEvent.Trigger(
                    MMInventoryEventType.ItemEquipped,
                    null,
                    TargetInventory.name,
                    item,
                    item.Quantity,
                    index,
                    PlayerID
                );

                Debug.Log($"Equipped {item.ItemID} from Hotbar.");
            }
        }


        /// <summary>
        ///     On Disable, we stop listening for MMInventoryEvents
        /// </summary>
        protected override void OnDisable()
        {
            base.OnDisable();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			HotbarInputAction.action.Disable();
#endif
        }
    }
}
