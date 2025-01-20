using System;
using System.Collections.Generic;

namespace Project.Core.CharacterCreation
{
    [Serializable]
    public class CharacterStats
    {
        public int health;
        public int stamina;
        public int strength;
        public int agility;
        public int endurance;
        public int intelligence;
        public int intuition;


        // Add other base stats as needed

        public Dictionary<string, int> GetModifiedStats(List<CharacterTrait> traits)
        {
            var modified = new Dictionary<string, int>();
            // Apply trait modifications
            return modified;
        }
    }
}
