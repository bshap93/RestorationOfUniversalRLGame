using Core.Events;
using UnityEngine;

namespace Gameplay.Character.Attributes.Dexterity
{
    public class SimplifiedDexterityManager : BasePlayerAttributeManager
    {
        public DexterityUIUpdater dexterityUIUpdater;
        protected override AttributeInQuestion AttributeType => AttributeInQuestion.Dexterity;
        protected override string SaveFileName => "PlayerDexterity";

        public override void ResetAttribute()
        {
            var characterStatProfile = PlayerAttributesProgressionManager.GetCharacterStatProfile();

            attributeLevel = characterStatProfile.InitialDexterityLevel;
            attributeExperiencePoints = characterStatProfile.InitialDexterityExperiencePoints;

            AttributeEvent.Trigger(AttributeType, AttributeEventType.Reset, attributeExperiencePoints);
            AttributeLevelEvent.Trigger(AttributeType, AttributeLevelEventType.Reset, attributeLevel);

            SaveAttribute();
        }

        public static void ResetPlayerDexterity()
        {
            var instance = FindFirstObjectByType<SimplifiedDexterityManager>();
            if (instance != null) instance.ResetAttribute();
            else Debug.LogError("No instance of SimplifiedDexterityManager found in the scene.");
        }
    }
}
