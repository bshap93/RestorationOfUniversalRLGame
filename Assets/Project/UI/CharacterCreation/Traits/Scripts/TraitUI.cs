using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Traits.Scripts
{
    public class TraitUI : MonoBehaviour
    {
        [Header("UI Elements")] [SerializeField]
        TextMeshProUGUI nameText;
        [SerializeField] Toggle selectionToggle;
        [SerializeField] Button infoButton;
        [SerializeField] Image traitIcon;
        [SerializeField] Image classSpecificIndicator;

        [Header("Styling")] [SerializeField] Color selectedColor = new(0.8f, 0.8f, 1f);
        [SerializeField] Color normalColor = Color.white;
        Action<CharacterTrait> onInfoRequested;
        Action<CharacterTrait, bool> onSelected;

        public CharacterTrait Trait { get; private set; }

        void OnDestroy()
        {
            // Clean up listeners
            if (selectionToggle != null) selectionToggle.onValueChanged.RemoveListener(OnToggleChanged);

            if (infoButton != null) infoButton.onClick.RemoveAllListeners();
        }

        public void Initialize(
            CharacterTrait traitData,
            Action<CharacterTrait, bool> onSelect,
            Action<CharacterTrait> onInfo,
            bool isClassSpecific)
        {
            Trait = traitData;
            onSelected = onSelect;
            onInfoRequested = onInfo;

            // Setup UI elements
            if (nameText != null)
            {
                nameText.text = Trait.traitName;
                nameText.fontStyle = FontStyles.Bold;
            }

            if (traitIcon != null && Trait.icon != null)
            {
                traitIcon.sprite = Trait.icon;
                traitIcon.preserveAspect = true;
            }

            // Setup class-specific indicator
            if (classSpecificIndicator != null) classSpecificIndicator.gameObject.SetActive(isClassSpecific);

            // Setup listeners
            if (selectionToggle != null) selectionToggle.onValueChanged.AddListener(OnToggleChanged);

            if (infoButton != null) infoButton.onClick.AddListener(OnInfoClicked);
        }

        void OnToggleChanged(bool isOn)
        {
            onSelected?.Invoke(Trait, isOn);
            UpdateVisuals(isOn);
        }

        void OnInfoClicked()
        {
            onInfoRequested?.Invoke(Trait);
        }

        void UpdateVisuals(bool isSelected)
        {
            // Update background color or any other visual feedback
            var image = GetComponent<Image>();
            if (image != null) image.color = isSelected ? selectedColor : normalColor;
        }

        public void SetToggleWithoutNotify(bool value)
        {
            if (selectionToggle != null)
            {
                selectionToggle.SetIsOnWithoutNotify(value);
                UpdateVisuals(value);
            }
        }
    }
}
