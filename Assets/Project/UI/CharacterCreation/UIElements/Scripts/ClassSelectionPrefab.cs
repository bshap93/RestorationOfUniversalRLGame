using System;
using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.UI.CharacterCreation.UIElements.Scripts
{
    public class ClassSelectionPrefab : MonoBehaviour
    {
        [SerializeField] TMP_Text className;

        [Header("Styling")] [SerializeField] Color selectedColor = new(0.8f, 0.8f, 1f);
        [SerializeField] Color normalColor = Color.white;
        [SerializeField] Image backgroundImage;
        [SerializeField] Button button;

        void Awake()
        {
            // Set up button colors
            var colors = button.colors;
            colors.normalColor = normalColor;
            colors.selectedColor = selectedColor;
            colors.highlightedColor = Color.Lerp(normalColor, selectedColor, 0.5f);
            button.colors = colors;
        }

        public void Setup(StartingClass classData, Action<StartingClass> onSelected)
        {
            if (classData == null) return;


            if (className != null)
            {
                className.text = classData.className;
                className.fontStyle = FontStyles.Bold;
            }


            if (button != null)
                button.onClick.AddListener(
                    () =>
                    {
                        onSelected?.Invoke(classData);
                        SetSelected(true);
                    });
        }

        public void SetSelected(bool selected)
        {
            if (backgroundImage != null) backgroundImage.color = selected ? selectedColor : normalColor;
        }
    }
}
