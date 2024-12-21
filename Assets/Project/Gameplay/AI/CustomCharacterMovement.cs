using TopDownEngine.Common.Scripts.Characters.CharacterAbilities;
using UnityEngine;

namespace Project.Gameplay.AI
{
    public class CustomCharacterMovement : CharacterMovement
    {
        public Vector2 GetCurrentInput()
        {
            return _currentInput;
        }
    }
}
