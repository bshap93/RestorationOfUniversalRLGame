using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Gameplay.Events;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.PickupDisplayer
{
    /// <summary>
    ///     A class used to display the picked up items on screen.
    ///     The PickupDisplayItems will be parented to it, so it's better if it has a LayoutGroup (Vertical or Horizontal) too.
    ///     </summadry>
    public class PickupDisplayer : MonoBehaviour, MMEventListener<ItemEvent>
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
            this.MMEventStartListening();
            OnValidate();
        }
        void OnDisable()
        {
            this.MMEventStopListening();
        }
        void OnValidate()
        {
            _pickupDisplayWfs = new WaitForSeconds(PickupDisplayDuration);
        }

        public void OnMMEvent(ItemEvent mmEvent)
        {
            if (mmEvent.EventName != "ItemPickedUp") return;
            Debug.LogError("PickupDisplayer: " + mmEvent.EventName + " x" + mmEvent.Amount);
            var item = mmEvent.Item;
            var quantity = mmEvent.Amount;
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
