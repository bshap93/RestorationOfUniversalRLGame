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
