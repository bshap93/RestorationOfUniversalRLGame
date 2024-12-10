using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Combat.Weapons
{
    public class CustomAmmoDisplay : AmmoDisplay, MMEventListener<MMInventoryEvent>, MMEventListener<MMGameEvent>
    {
        [FormerlySerializedAs("TotalAmmoTextDisplay")]
        [MMInspectorGroup("Total Ammo in Main Inventory", true, 12)]
        [Tooltip("Total Ammo in Main Inventory")]
        public TMP_Text totalAmmoTextDisplay;
        [FormerlySerializedAs("AmmoInventory")]
        public Inventory ammoInventory;
        [FormerlySerializedAs("AmmoID")] public string ammoID;
        /// the current amount of ammo available in the inventory
        [FormerlySerializedAs("CurrentAmmoAvailable")]
        [MMReadOnly]
        [Tooltip("the current amount of ammo available in the inventory")]
        public int currentAmmoAvailable;

        protected override void OnEnable()
        {
            base.OnEnable();
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening<MMGameEvent>();
            this.MMEventStartListening<MMInventoryEvent>();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening<MMGameEvent>();
            this.MMEventStopListening<MMInventoryEvent>();
        }
        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "FiredProjectile") RefreshCurrentAmmoAvailable();
        }

        public void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.InventoryEventType is MMInventoryEventType.ItemEquipped)
            {
                gameObject.SetActive(true);
                RefreshCurrentAmmoAvailable();
                Debug.Log("Item Equipped");
            }
        }


        protected virtual void RefreshCurrentAmmoAvailable()
        {
            currentAmmoAvailable = ammoInventory.GetQuantity(ammoID);
            totalAmmoTextDisplay.text = currentAmmoAvailable.ToString();
            Debug.Log("Current Ammo: " + currentAmmoAvailable);
        }
    }
}
