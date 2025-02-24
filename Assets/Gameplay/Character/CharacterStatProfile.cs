using UnityEngine;

namespace Gameplay.Character
{
    [CreateAssetMenu(fileName = "CharacterStatProfile", menuName = "Character/Character Stat Profile")]
    public class CharacterStatProfile : ScriptableObject
    {
        public float InitialMaxStamina;
        public float InitialMaxHealth;
    }
}
