using Project.Core.CharacterCreation;
using TMPro;
using UnityEngine;

namespace Project.UI.CharacterCreation.UIElements.Scripts
{
    public class ConfirmationPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI classSummaryText;
        [SerializeField] TextMeshProUGUI attributesSummaryText;
        [SerializeField] TextMeshProUGUI traitsSummaryText;

        public void DisplayCharacterSummary(CharacterCreationData data)
        {
            // Display class
            classSummaryText.text = $"Class: {data.selectedClassName}";

            // Display attributes
            attributesSummaryText.text = "Attributes:\n" +
                                         $"Strength: {data.attributes.strength}\n" +
                                         $"Agility: {data.attributes.agility}\n" +
                                         $"Endurance: {data.attributes.endurance}\n" +
                                         $"Intelligence: {data.attributes.intelligence}\n" +
                                         $"Intuition: {data.attributes.intuition}";

            // Display traits
            var traitNames = data.selectedTraitNames;
            traitsSummaryText.text = "Selected Traits:\n" + string.Join("\n", traitNames);
        }
    }
}
