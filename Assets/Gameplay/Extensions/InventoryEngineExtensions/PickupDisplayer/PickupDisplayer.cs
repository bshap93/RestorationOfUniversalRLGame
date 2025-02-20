using System.Collections;
using System.Collections.Generic;
using Gameplay.Events;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.PickupDisplayer
{
    /// <summary>
    ///     A class used to display the picked up items on screen.
    ///     The PickupDisplayItems will be parented to it, so it's better if it has a LayoutGroup (Vertical or Horizontal) too.
    ///     </summadry>
    public class PickupDisplayer : MonoBehaviour, MMEventListener<ItemEvent>, MMEventListener<MMInventoryEvent>
    {
        [Tooltip("the prefab to use to display achievements")]
        public PickupDisplayItem PickupDisplayPrefab;
        [Tooltip("the duration the pickup display item will remain on screen")]
        public float PickupDisplayDuration = 5;
        [Tooltip("the fade in/out duration")] public float PickupFadeDuration = .2f;
        readonly Dictionary<string, PickupDisplayItem> _displays = new();
        WaitForSeconds _pickupDisplayWfs;
        void OnEnable()
        {
            this.MMEventStartListening<ItemEvent>();

            this.MMEventStartListening<MMInventoryEvent>();

            OnValidate();
        }
        void OnDisable()
        {
            this.MMEventStopListening<ItemEvent>();

            this.MMEventStopListening<MMInventoryEvent>();
        }
        void OnValidate()
        {
            _pickupDisplayWfs = new WaitForSeconds(PickupDisplayDuration);
        }

        public void OnMMEvent(ItemEvent mmEvent)
        {
            if (mmEvent.EventName != "ItemPickedUp") return;
            var item = mmEvent.Item;
            var quantity = mmEvent.Amount;
            DisplayPickedItem(item, quantity);
        }
        public void OnMMEvent(MMInventoryEvent eventType)
        {
            if (eventType.InventoryEventType == MMInventoryEventType.Pick)
                DisplayPickedItem(eventType.EventItem, eventType.Quantity);
            
            
        }
        void DisplayPickedItem(InventoryItem item, int quantity)
        {
            if (_displays.TryGetValue(item.ItemID, out var display))
            {
                display.AddQuantity(quantity);
            }
            else
            {
                _displays[item.ItemID] = Instantiate(PickupDisplayPrefab, transform);
                _displays[item.ItemID].Display(item, quantity);
                var canvasGroup = _displays[item.ItemID].GetComponent<CanvasGroup>();
                if (canvasGroup)
                {
                    canvasGroup.alpha = 0;
                    StartCoroutine(MMFade.FadeCanvasGroup(canvasGroup, PickupFadeDuration, 1));
                }

                StartCoroutine(FadeOutAndDestroy());

                IEnumerator FadeOutAndDestroy()
                {
                    yield return _pickupDisplayWfs;
                    if (canvasGroup) yield return MMFade.FadeCanvasGroup(canvasGroup, PickupFadeDuration, 0);
                    Destroy(_displays[item.ItemID].gameObject);
                    _displays.Remove(item.ItemID);
                }
            }
        }
    }
}
