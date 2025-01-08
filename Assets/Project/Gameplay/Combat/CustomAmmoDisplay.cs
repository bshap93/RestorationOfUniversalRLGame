using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using TopDownEngine.Common.Scripts.GUI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Project.Gameplay.Combat
{
    public class CustomAmmoDisplay : AmmoDisplay, MMEventListener<MMInventoryEvent>, MMEventListener<MMGameEvent>
    {
        [FormerlySerializedAs("TotalAmmoTextDisplay")]
        [MMInspectorGroup("Total Ammo in Main Inventory", true, 12)]
        [Tooltip("Total Ammo in Main Inventory")]
        public Text totalAmmoTextDisplay;
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
        public void OnMMEvent(MMGameEvent recipeEvent)
        {
            if (recipeEvent.EventName == "FiredProjectile") RefreshCurrentAmmoAvailable();
        }

        public void OnMMEvent(MMInventoryEvent recipeEvent)
        {
            if (recipeEvent.InventoryEventType is MMInventoryEventType.ItemEquipped)
            {
                gameObject.SetActive(true);
                RefreshCurrentAmmoAvailable();
                Debug.Log("Item Equipped");
            }
        }


        protected virtual void RefreshCurrentAmmoAvailable()
        {
            if (ammoInventory == null) return;
            currentAmmoAvailable = ammoInventory.GetQuantity(ammoID);
            totalAmmoTextDisplay.text = currentAmmoAvailable.ToString();
            Debug.Log("Current Ammo: " + currentAmmoAvailable);
        }
    }
}
