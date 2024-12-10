using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    public class CustomAmmoDisplay : AmmoDisplay, MMEventListener<MMInventoryEvent>
    {
        [MMInspectorGroup("Total Ammo in Main Inventory", true, 12)]
        /// the ID of the AmmoDisplay 
        [Tooltip("Total Ammo in Main Inventory")]
        public TMP_Text TotalAmmoTextDisplay;
        public Inventory AmmoInventory;
        public string AmmoID;
        /// the current amount of ammo available in the inventory
        [MMReadOnly] [Tooltip("the current amount of ammo available in the inventory")]
        public int CurrentAmmoAvailable;

        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.InventoryEventType is MMInventoryEventType.ItemEquipped)
            {
                gameObject.SetActive(true);
                RefreshCurrentAmmoAvailable();
                Debug.Log("Item Equipped");
            }

            if (eventType.InventoryEventType is MMInventoryEventType.ItemUnEquipped) gameObject.SetActive(false);
        }


        protected virtual void RefreshCurrentAmmoAvailable()
        {
            CurrentAmmoAvailable = AmmoInventory.GetQuantity(AmmoID);
            TotalAmmoTextDisplay.text = CurrentAmmoAvailable.ToString();
            Debug.Log("Current Ammo: " + CurrentAmmoAvailable);
        }
    }
}
