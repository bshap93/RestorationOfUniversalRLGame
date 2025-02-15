using System.Collections;
using System.Collections.Generic;
using MoreMountains.InventoryEngine;
using MoreMountains.Tools;
using TMPro;
using UnityEngine;

namespace Prefabs.UI.PrefabRequiredScripts
{
    public class TMPInventoryDetails : InventoryDetails
    {
        public enum DisplayMode
        {
            Inventory,
            Preview
        }

        public List<string> TargetInventoryNames = new();

        public DisplayMode CurrentMode = DisplayMode.Inventory;

        [Header("TMP Components")] public TMP_Text TMPTitle;
        public TMP_Text TMPShortDescription;
        public TMP_Text TMPDescription;
        public TMP_Text TMPQuantity;
        public TMP_Text TMPWeight;
        public string DefaultWeight = "0.0";


        protected virtual void OnValidate()
        {
            if (TMPTitle != null && Title != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for Title. Only TMP_Text will be used.");

            if (TMPShortDescription != null && ShortDescription != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for ShortDescription. Only TMP_Text will be used.");

            if (TMPDescription != null && Description != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for Description. Only TMP_Text will be used.");

            if (TMPQuantity != null && Quantity != null)
                Debug.LogWarning(
                    "Both Text and TMP_Text components are assigned for Quantity. Only TMP_Text will be used.");
        }

        protected virtual float GetItemWeight(InventoryItem item)
        {
            if (item == null) return 0f;
            var weightProperty = item.GetType().GetProperty("Weight");
            if (weightProperty != null)
                try
                {
                    return (float)weightProperty.GetValue(item, null);
                }
                catch
                {
                    return 0f;
                }

            return 0f;
        }

        protected override IEnumerator FillDetailFields(InventoryItem item, float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);

            if (CurrentMode == DisplayMode.Inventory)
            {
                if (TMPTitle != null) TMPTitle.text = item.ItemName;
                if (TMPShortDescription != null) TMPShortDescription.text = item.ShortDescription;
                if (TMPDescription != null) TMPDescription.text = item.Description;
                if (TMPQuantity != null) TMPQuantity.text = item.Quantity.ToString();
                if (Icon != null) Icon.sprite = item.Icon;

                if (TMPWeight != null)
                {
                    var itemWeight = GetItemWeight(item);
                    TMPWeight.text = (itemWeight * item.Quantity).ToString("F1");
                }

                if (HideOnEmptySlot && !Hidden && item.Quantity == 0)
                {
                    StartCoroutine(MMFade.FadeCanvasGroup(_canvasGroup, _fadeDelay, 0f));
                    Hidden = true;
                }
            }
        }

        /// <summary>
        ///     Catches MMInventoryEvents and displays details if needed
        /// </summary>
        public override void OnMMEvent(MMInventoryEvent mmEvent)
        {
            // If Global is enabled, listen to all events regardless of inventory name
            if (Global)
            {
                base.OnMMEvent(mmEvent);
                return;
            }

            // Check if the event's TargetInventoryName is in our list of target inventory names
            if (!TargetInventoryNames.Contains(mmEvent.TargetInventoryName)) return;

            if (mmEvent.PlayerID != PlayerID) return;

            switch (mmEvent.InventoryEventType)
            {
                case MMInventoryEventType.Select:
                    DisplayDetails(mmEvent.EventItem);
                    break;
                case MMInventoryEventType.UseRequest:
                    DisplayDetails(mmEvent.EventItem);
                    break;
                case MMInventoryEventType.InventoryOpens:
                    DisplayDetails(mmEvent.EventItem);
                    break;
                case MMInventoryEventType.Drop:
                    DisplayDetails(null);
                    break;
                case MMInventoryEventType.EquipRequest:
                    DisplayDetails(null);
                    break;
            }
        }

        protected override IEnumerator FillDetailFieldsWithDefaults(float initialDelay)
        {
            yield return new WaitForSeconds(initialDelay);

            if (TMPTitle != null) TMPTitle.text = DefaultTitle;
            if (TMPShortDescription != null) TMPShortDescription.text = DefaultShortDescription;
            if (TMPDescription != null) TMPDescription.text = DefaultDescription;
            if (TMPQuantity != null) TMPQuantity.text = DefaultQuantity;
            if (TMPWeight != null) TMPWeight.text = DefaultWeight;
            if (Icon != null) Icon.sprite = DefaultIcon;
        }

        public void DisplayPreview(InventoryItem item)
        {
            CurrentMode = DisplayMode.Preview;
            if (item == null) return;

            if (TMPTitle != null) TMPTitle.text = item.ItemName;
            if (TMPShortDescription != null) TMPShortDescription.text = item.ShortDescription;
            if (TMPDescription != null) TMPDescription.text = item.Description;
            if (TMPQuantity != null) TMPQuantity.text = item.Quantity.ToString();
            if (Icon != null) Icon.sprite = item.Icon;


            if (_canvasGroup != null) _canvasGroup.alpha = 1;
        }

        public void HidePreview()
        {
            if (_canvasGroup != null) _canvasGroup.alpha = 0;
            CurrentMode = DisplayMode.Inventory;
        }
    }
}
