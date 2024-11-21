using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation
{
    [Serializable]
    public class CharacterCreationData
    {
        public string characterName;
        public List<string> selectedTraitNames = new();
        public int remainingPoints;
        public string selectedClassName;
        public CharacterStats attributes = new();
    }

    [Serializable]
    public enum StatType
    {
        Strength,
        Agility,
        Endurance,
        Intelligence,
        Intuition
    }

    [CreateAssetMenu(fileName = "New Starting Class", menuName = "Roguelike/Starting Class")]
    public class StartingClass : ScriptableObject
    {
        // Keep existing fields (don't modify these)
        public string className; // Keep for backwards compatibility
        public string description;
        public Sprite classIcon;
        public List<CharacterTrait> defaultTraits = new();
        public Dictionary<StatType, int> baseStats = new();

        // Add new properties/methods that work with existing data
        public CharacterClass ClassType
        {
            get
            {
                // Parse the existing className to get enum value
                if (Enum.TryParse<CharacterClass>(className, out var result)) return result;
                Debug.LogError($"Invalid class name {className} in {name}");
                return default;
            }
        }
    }

    public enum CharacterClass
    {
        Automaton,
        Zealot
    }


    [CreateAssetMenu(fileName = "New Trait", menuName = "Roguelike/Character Trait")]
    public class CharacterTrait : ScriptableObject
    {
        public enum ModifierType
        {
            Additive,
            Multiplicative
        }

        public string traitName;
        public string description;
        public Sprite icon;
        public List<StatModifier> statModifiers = new();
        public List<string> specialEffects = new();

        [Header("Class Restrictions")] public bool isClassSpecific;
        [Tooltip("If class specific, which classes can use this trait")]
        public List<CharacterClass> availableForClasses = new();

        public bool IsAvailableForClass(CharacterClass characterClass)
        {
            return !isClassSpecific || availableForClasses.Contains(characterClass);
        }

        [Serializable]
        public class StatModifier
        {
            public string statName;
            public float value;
            public ModifierType type;
        }
    }


    [Serializable]
    public class StatModifier
    {
        public string statName;
        public int value;
    }
}
