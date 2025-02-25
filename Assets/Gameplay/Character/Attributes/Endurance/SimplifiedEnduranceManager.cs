using Core.Events;
using UnityEngine;

namespace Gameplay.Character.Attributes.Endurance
{
    public class SimplifiedEnduranceManager : BasePlayerAttributeManager
    {
        public EnduranceUIUpdater EnduranceUIUpdater;

        protected override AttributeInQuestion AttributeType => AttributeInQuestion.Endurance;
        protected override string SaveFileName => "PlayerEndurance";

        public override void ResetAttribute()
        {
            var characterStatProfile = PlayerAttributesProgressionManager.GetCharacterStatProfile();

            attributeLevel = characterStatProfile.InitialEnduranceLevel;

            attributeExperiencePoints = characterStatProfile.InitialEnduranceExperiencePoints;

            AttributeEvent.Trigger(AttributeType, AttributeEventType.Reset, attributeExperiencePoints);

            AttributeLevelEvent.Trigger(AttributeType, AttributeLevelEventType.Reset, attributeLevel);

            SaveAttribute();
        }

        public static void ResetPlayerEndurance()
        {
            var instance = FindFirstObjectByType<SimplifiedEnduranceManager>();
            if (instance != null) instance.ResetAttribute();
            else Debug.LogError("No instance of SimplifiedEnduranceManager found in the scene.");
        }
    }
}
