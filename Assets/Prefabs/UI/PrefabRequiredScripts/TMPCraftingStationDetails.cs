using MoreMountains.Tools;
using Project.Gameplay.Interactivity.CraftingStation;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Prefabs.UI.PrefabRequiredScripts
{
    public class TMPCraftingStationDetails : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        [Header("Crafting Station Details")] public TMP_Text TMPTitle;
        public TMP_Text TMPShortDescription;
        public TMP_Text TMPDescription;
        public TMP_Text TMPEfficiency;
        public TMP_Text TMPConcurrentCraftingLimit;
        public TMP_Text TMPIsActive;
        public Image Icon;

        [Header("Defaults")] public string DefaultTitle = "No Station Selected";
        public string DefaultShortDescription = "Select a crafting station";
        public string DefaultDescription = "";
        public string DefaultEfficiency = "0.0";
        public string DefaultConcurrentCraftingLimit = "0";
        public string DefaultIsActive = "Inactive";
        public Sprite DefaultIcon;

        [FormerlySerializedAs("PreviewEventNamae")]
        [FormerlySerializedAs("PreviewCraftingStation")]
        [FormerlySerializedAs("CraftingStationSelectedEvent")]
        public string PreviewEventName = "PreviewCraftingStation";
        public CraftingStationType CraftingStationType;

        CanvasGroup _canvasGroup;


        void Start()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            gameObject.SetActive(false);
        }

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent mmEvent)
        {
            if (mmEvent.EventName == PreviewEventName)
            {
                switch (CraftingStationType)
                {
                    case CraftingStationType.CookingStation:
                        break;
                }

                gameObject.SetActive(true);
            }
        }


        /// <summary>
        ///     Fills the fields with default values.
        /// </summary>
        public void FillWithDefaults()
        {
            if (TMPTitle != null) TMPTitle.text = DefaultTitle;
            if (TMPShortDescription != null) TMPShortDescription.text = DefaultShortDescription;
            if (TMPDescription != null) TMPDescription.text = DefaultDescription;
            if (TMPEfficiency != null) TMPEfficiency.text = DefaultEfficiency;
            if (TMPConcurrentCraftingLimit != null) TMPConcurrentCraftingLimit.text = DefaultConcurrentCraftingLimit;
            if (TMPIsActive != null) TMPIsActive.text = DefaultIsActive;
            if (Icon != null) Icon.sprite = DefaultIcon;

            Show();
        }

        /// <summary>
        ///     Hides the details panel.
        /// </summary>
        public void Hide()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        ///     Shows the details panel.
        /// </summary>
        public void Show()
        {
            if (_canvasGroup != null)
            {
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
            }
        }
    }
}
