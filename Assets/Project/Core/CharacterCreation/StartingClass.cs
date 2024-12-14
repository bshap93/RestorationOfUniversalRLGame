using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Core.CharacterCreation
{
    [Serializable]
    public class Buff
    {
        public string buffName;
        public string description;
        public List<CharacterTrait.StatModifier> statModifiers;
    }

    [CreateAssetMenu(fileName = "New Starting Class", menuName = "Roguelike/Starting Class")]
    public class StartingClass : ScriptableObject
    {
        // Keep existing fields (don't modify these)
        public string className; // Keep for backwards compatibility
        public string description;
        public Sprite classIcon;
        public List<CharacterTrait> defaultTraits = new();
        public List<Buff> startingBuffs;
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
}
