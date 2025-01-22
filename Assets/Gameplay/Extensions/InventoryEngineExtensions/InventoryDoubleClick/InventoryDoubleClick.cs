using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace InventoryDoubleClick
{
    public class InventoryDoubleClick : MonoBehaviour, MMEventListener<MMInventoryEvent>
    {
        const float _doubleClickMaxDelay = .5f;
        bool _clicked;
        float _clickedTime;
        InventorySlot _slot;
        bool DoubleClick => _clicked && Time.unscaledTime - _clickedTime < _doubleClickMaxDelay;

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
            if (mmEvent.InventoryEventType != MMInventoryEventType.Click) return;
            if (!DoubleClick)
            {
                _clicked = true;
                _clickedTime = Time.unscaledTime;
                _slot = mmEvent.Slot;
            }
            else
            {
                _clicked = false;
                if (mmEvent.Slot != _slot) return;
                if (_slot.Unequippable()) _slot.UnEquip();
                else if (_slot.Equippable()) _slot.Equip();

                if (_slot.Usable()) _slot.Use();
            }
        }
    }
}
