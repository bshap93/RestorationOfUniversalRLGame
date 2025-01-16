using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using UnityEngine.Serialization;

public class EquipmentInventoryManager : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    public List<InventoryItem> TwoHandedItems = new();
    public List<InventoryItem> LeftHandedItems = new();
    public List<InventoryItem> RightHandedItems = new();
    public InventorySlot RightHandSlot;
    public InventorySlot LeftHandSlot;

    [FormerlySerializedAs("_isTwoHandedWeaponEquipped")] [SerializeField]
    bool isTwoHandedWeaponEquipped;

    public void OnMMEvent(MMInventoryEvent @event)
    {
        switch (@event.InventoryEventType)
        {
            case MMInventoryEventType.EquipRequest:
                var item = @event.EventItem;
                Debug.Log("Item requested equipment: " + item.ItemID);
                if (TwoHandedItems.Contains(item))
                {
                    // If a two-handed weapon is equipped, unequip the left hand slot
                    // For now we use the right hand slot as the two-handed weapon slot
                    LeftHandSlot.UnEquip();
                    isTwoHandedWeaponEquipped = true;
                }

                if (LeftHandedItems.Contains(item))
                    if (isTwoHandedWeaponEquipped)
                    {
                        RightHandSlot.UnEquip();
                        isTwoHandedWeaponEquipped = false;
                    }

                if (RightHandedItems.Contains(item))
                    if (isTwoHandedWeaponEquipped)
                        isTwoHandedWeaponEquipped = false;


                break;
            case MMInventoryEventType.UnEquipRequest:
                break;
        }
    }
}
