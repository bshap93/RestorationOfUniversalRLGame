using System.Collections.Generic;
using MoreMountains.InventoryEngine;

namespace Project.Gameplay.ItemManagement
{
    public class CustomInventoryInputManager : InventoryInputManager
    {
        int _hotbarInputKeyPressedIndex;
        protected List<CustomInventoryHotbar> _targetCustomInventoryHotbars;
        // protected bool _hotbarInputPressed = false; 

        /// <summary>
        ///     On start, we grab references and prepare our hotbar list
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _targetCustomInventoryHotbars = new List<CustomInventoryHotbar>();
            foreach (var go in FindObjectsOfType(typeof(CustomInventoryHotbar)) as CustomInventoryHotbar[])
                _targetCustomInventoryHotbars.Add(go);
        }


        protected override void HandleHotbarsInput()
        {
            if (!InventoryIsOpen)

                foreach (var hotbar in _targetCustomInventoryHotbars)
                    if (hotbar != null)
                    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
						_hotbarInputPressed = hotbar.HotbarInputAction.action.WasPressedThisFrame();
#else
                        _hotbarInputPressed = false;
                        for (var i = 0; i < hotbar.HotbarKeys.Length; i++)
                            if (UnityEngine.Input.GetKeyDown(hotbar.HotbarKeys[i]) ||
                                UnityEngine.Input.GetKeyDown(hotbar.HotbarAltKeys[i]))
                            {
                                _hotbarInputPressed = true;
                                _hotbarInputKeyPressedIndex = i;
                            }
#endif

                        if (_hotbarInputPressed) hotbar.Action(_hotbarInputKeyPressedIndex);
                    }
        }
    }
}
