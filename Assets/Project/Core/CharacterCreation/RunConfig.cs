using System;
using System.Collections.Generic;

namespace Project.Core.CharacterCreation
{
    [Serializable]
    public class RunConfig
    {
        public int seed; // For run randomization
        public CharacterStats baseStats; // Base character stats
        public List<CharacterTrait> traits; // Chosen character traits
        public int startingGold; // Starting resources
        public List<string> startingItems; // Starting inventory
        public int attributePointsRemaining; // Points to distribute
        public StartingClass startingClass { get; set; }

        public void ApplyAttributePoints(string attribute, int points)
        {
            // Apply points to specific attributes
        }
    }
}
