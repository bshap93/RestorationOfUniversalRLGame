// CharacterSaveData.cs

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation.SaveData
{
    [CreateAssetMenu(fileName = "NewCharacterSave", menuName = "Roguelike/Character Save")]
    public class CharacterSaveData : ScriptableObject
    {
        public string characterName;
        public string selectedClassName; // Updated to match CharacterCreationData
        public List<string> selectedTraitNames = new(); // Updated to match CharacterCreationData
        public CharacterStats attributes;

        // Runtime references to the actual ScriptableObject data
        [NonSerialized] public StartingClass selectedClass;
        [NonSerialized] public List<CharacterTrait> selectedTraits;

        public void Initialize(CharacterCreationData creationData)
        {
            characterName = creationData.characterName;
            selectedClassName = creationData.selectedClassName; // Store class name
            selectedTraitNames = new List<string>(creationData.selectedTraitNames); // Store trait names
            attributes = creationData.attributes;
        }

        public void LoadData()
        {
            // Load StartingClass by selectedClassName
            selectedClass = Resources.Load<StartingClass>($"Classes/{selectedClassName}");
            if (selectedClass == null) Debug.LogError($"Class {selectedClassName} not found in resources.");

            // Load CharacterTrait objects by selectedTraitNames
            selectedTraits = new List<CharacterTrait>();
            foreach (var traitName in selectedTraitNames)
            {
                var trait = Resources.Load<CharacterTrait>($"Traits/{traitName}");
                if (trait != null)
                    selectedTraits.Add(trait);
                else
                    Debug.LogError($"Trait {traitName} not found in resources.");
            }

            Debug.Log(
                $"Loaded CharacterSaveData for {characterName} with Class: {selectedClassName} and Traits: {string.Join(", ", selectedTraitNames)}");
        }
    }
}
