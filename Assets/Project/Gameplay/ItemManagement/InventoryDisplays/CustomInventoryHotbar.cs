using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.ItemManagement
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
        void Start()
        {
        }

        /// <summary>
        ///     Executed when the key or alt key gets pressed, triggers the specified action
        /// </summary>
        public virtual void Action(int index)
        {
            if (!InventoryItem.IsNull(TargetInventory.Content[index]))
            {
                var item = TargetInventory.Content[index];
                Debug.Log($"Item in slot {index} is {item.ItemID}");
                if (item.Equippable)
                {
                    // item.Equip(PlayerID);
                    InventorySlots[index].Equip();
                    Debug.Log($"Equipped {item.ItemID}");
                }

                if (item.Usable)
                {
                    // item.Use(PlayerID);
                    InventorySlots[index].Use();
                    Debug.Log($"Used {item.ItemID}");
                }
            }
        }

        /// <summary>
        ///     On Enable, we start listening for MMInventoryEvents
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
			HotbarInputAction.action.Enable();
#endif
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
