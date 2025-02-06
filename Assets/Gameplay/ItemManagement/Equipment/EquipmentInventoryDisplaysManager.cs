using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.ItemManagement
{
    public class EquipmentInventoryDisplaysManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        public List<InventoryItem> TwoHandedItems = new();
        public List<InventoryItem> LeftHandedItems = new();
        public List<InventoryItem> RightHandedItems = new();
        public InventorySlot RightHandSlot;
        public InventorySlot LeftHandSlot;

        [FormerlySerializedAs("_isTwoHandedWeaponEquipped")] [SerializeField]
        bool isTwoHandedWeaponEquipped;


        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMInventoryEvent mmEvent)
        {
            switch (mmEvent.InventoryEventType)
            {
                case MMInventoryEventType.EquipRequest:
                    var item = mmEvent.EventItem;
                    Debug.Log("Item requested equipment: " + item.ItemID);

                    // Ignore Two-Handed and Left/Right Hand logic since we're using the hotbar
                    break;

                case MMInventoryEventType.ItemUnEquipped:
                    // Prevent unnecessary unequip
                    break;
            }
        }
    }
}
