using System;

namespace Project.Editor.OdinAttributes
{
    public class StaminaBarAttribute : Attribute
    {
        public float MaxStamina;

        public StaminaBarAttribute(float maxStamina)
        {
            MaxStamina = maxStamina;
        }
    }
}
