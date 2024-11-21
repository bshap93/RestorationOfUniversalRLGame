using Project.Core.CharacterCreation;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class MagicSystem : MonoBehaviour
    {
        public MagicResource Kinema; // Kinema resource
        public MagicResource Favour; // Favour resource

        public CharacterClass characterClass;

        public MagicResource PrimaryResource => Kinema;
        public MagicResource SecondaryResource => Favour;

        public void ReceiveKinema(float kinema, GameObject instigator)
        {
            Kinema.RestoreResource(kinema);
        }

        public void ReceiveFavour(float favour, GameObject instigator)
        {
            Favour.RestoreResource(favour);
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
