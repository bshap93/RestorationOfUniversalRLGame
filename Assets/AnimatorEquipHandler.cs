using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

public class AnimatorEquipHandler : MonoBehaviour, MMEventListener<MMInventoryEvent>
{
    Animator _playerAnimator;

    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }

    public void OnEnable()
    {
        this.MMEventStartListening();
    }

    public void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMInventoryEvent eventType)
    {
        if (eventType.InventoryEventType == MMInventoryEventType.ItemEquipped)
        {
        }
        else if (eventType.InventoryEventType == MMInventoryEventType.ItemUnEquipped)
        {
        }
    }
}
