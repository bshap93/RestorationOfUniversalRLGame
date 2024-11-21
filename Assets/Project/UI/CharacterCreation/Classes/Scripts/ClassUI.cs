using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.Classes.Scripts
{
    public class ClassUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText;
        [SerializeField] Image classIcon;
        [SerializeField] Toggle selectionToggle;

        [SerializeField] Image classSpecificIndicator; // Add visual indicator for class-specific traits
        [SerializeField] Button infoButton;
        Action<StartingClass> onInfoRequested;
        Action<StartingClass> onSelected;


        public StartingClass StartingClass { get; private set; }

        public void Initialize(
            StartingClass classData,
            Action<StartingClass> onSelect,
            Action<StartingClass> onInfo)
        {
            StartingClass = classData;
            onSelected = onSelect;
            onInfoRequested = onInfo;

            // Setup UI elements
            nameText.text = StartingClass.className;
            if (StartingClass.classIcon != null)
                classIcon.sprite = StartingClass.classIcon;

            // Setup listeners
            selectionToggle.onValueChanged.AddListener(OnToggleChanged);
            infoButton.onClick.AddListener(OnInfoClicked);
        }

        void OnToggleChanged(bool isOn)
        {
            onSelected?.Invoke(StartingClass);
        }

        void OnInfoClicked()
        {
            onInfoRequested?.Invoke(StartingClass);
        }
    }
}
