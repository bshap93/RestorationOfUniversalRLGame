using MoreMountains.InventoryEngine;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Gameplay.ItemManagement.UI
{
    public class RightClickHandler : MonoBehaviour, IPointerClickHandler
    {
        CustomDepositInventoryDisplay _customDepositInventoryDisplay;

        void Start()
        {
            _customDepositInventoryDisplay = gameObject.GetComponentInParent<CustomDepositInventoryDisplay>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && _customDepositInventoryDisplay != null)
            {
                var slot = GetComponent<InventorySlot>();
                _customDepositInventoryDisplay.OnRightClick(slot);
            }
        }
    }
}
