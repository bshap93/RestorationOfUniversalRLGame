using UnityEngine;

namespace Gameplay.Character
{
    [CreateAssetMenu(fileName = "CharacterStatProfile", menuName = "Character/Character Stat Profile")]
    public class CharacterStatProfile : ScriptableObject
    {
        [Header("Initial Stats")] public float InitialMaxStamina;
        public float InitialMaxHealth;
        [Header("Initial Dexterity")] public int InitialDexterityLevel;
        public int InitialDexterityExperiencePoints;
        public int InitialDexterityExperiencePointsToNextLevel;

        [Header("Initial Endurance")] public int InitialEnduranceLevel;
        public int InitialEnduranceExperiencePoints;
        public int InitialEnduranceExperiencePointsToNextLevel;
    }
}
