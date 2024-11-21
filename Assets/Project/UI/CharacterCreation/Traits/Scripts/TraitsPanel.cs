using System.Collections.Generic;
using System.Linq;
using Project.Core.CharacterCreation;
using Project.UI.CharacterCreation.Traits.Scripts;
using TMPro;
using UnityEngine;

namespace Project.UI.CharacterCreation.Scripts
{
    public class TraitsPanel : MonoBehaviour
    {
        [Header("UI Elements")] [SerializeField]
        Transform traitContainer; // Where trait buttons spawn
        [SerializeField] GameObject traitPrefab;
        [SerializeField] TextMeshProUGUI descriptionText;
        [SerializeField] TextMeshProUGUI selectionCounterText;

        [Header("Settings")] [SerializeField] int maxTraitSelections = 2;
        CharacterClass currentClass;
        readonly List<CharacterTrait> selectedTraits = new();

        // Private state
        readonly List<TraitUI> traitUIs = new();

        public void Initialize(List<CharacterTrait> availableTraits, CharacterClass characterClass)
        {
            currentClass = characterClass;
            ClearTraits();

            // Only show traits available for this class
            var validTraits = availableTraits
                .Where(t => t.IsAvailableForClass(characterClass))
                .ToList();

            foreach (var trait in validTraits)
            {
                var traitGO = Instantiate(traitPrefab, traitContainer);
                var traitUI = traitGO.GetComponent<TraitUI>();

                traitUI.Initialize(
                    trait,
                    OnTraitSelected,
                    OnTraitInfoRequested,
                    trait.isClassSpecific
                );

                traitUIs.Add(traitUI);
            }

            UpdateSelectionCounter();
        }

        void OnTraitSelected(CharacterTrait trait, bool isSelected)
        {
            if (isSelected)
            {
                if (selectedTraits.Count < maxTraitSelections)
                {
                    selectedTraits.Add(trait);
                }
                else
                {
                    // Too many traits selected, uncheck the toggle
                    var traitUI = traitUIs.Find(ui => ui.Trait == trait);
                    traitUI.SetToggleWithoutNotify(false);
                    return;
                }
            }
            else
            {
                selectedTraits.Remove(trait);
            }

            UpdateSelectionCounter();
        }

        void UpdateSelectionCounter()
        {
            if (selectionCounterText != null)
                selectionCounterText.text = $"Selected: {selectedTraits.Count}/{maxTraitSelections}";
        }

        void OnTraitInfoRequested(CharacterTrait trait)
        {
            if (descriptionText != null)
            {
                var description = $"{trait.traitName}\n\n{trait.description}";

                // Add stat modifications if any exist
                if (trait.statModifiers.Any())
                {
                    description += "\n\nModifies:";
                    foreach (var mod in trait.statModifiers)
                    {
                        var prefix = mod.value >= 0 ? "+" : "";
                        description += $"\n{mod.statName}: {prefix}{mod.value}";
                    }
                }

                descriptionText.text = description;
            }
        }

        public List<CharacterTrait> GetSelectedTraits()
        {
            return new List<CharacterTrait>(selectedTraits);
        }

        public bool HasRequiredTraits()
        {
            return selectedTraits.Count == maxTraitSelections;
        }

        void ClearTraits()
        {
            foreach (var traitUI in traitUIs)
                if (traitUI != null)
                    Destroy(traitUI.gameObject);

            traitUIs.Clear();
            selectedTraits.Clear();

            if (descriptionText != null) descriptionText.text = "Select a trait to view its description";

            UpdateSelectionCounter();
        }
    }
}
