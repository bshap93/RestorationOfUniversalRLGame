using Project.Core.CharacterCreation;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class MagicSystem : MonoBehaviour
    {
        public MagicResource Kinema; // Kinema resource
        public MagicResource Favour; // Favour resource

        public CharacterClass characterClass;

        public MagicResource PrimaryResource => characterClass == CharacterClass.Automaton ? Kinema : Favour;
        public MagicResource SecondaryResource => characterClass == CharacterClass.Automaton ? Favour : Kinema;

        void Start()
        {
            // Optionally initialize resources here
        }

        public bool CanConsumePrimary(float amount)
        {
            return PrimaryResource.CurrentResource >= amount;
        }

        public bool CanConsumeSecondary(float amount)
        {
            return SecondaryResource.CurrentResource >= amount;
        }

        public void ConsumePrimary(float amount)
        {
            PrimaryResource.ConsumeResource(amount);
        }

        public void ConsumeSecondary(float amount)
        {
            SecondaryResource.ConsumeResource(amount);
        }
    }
}
