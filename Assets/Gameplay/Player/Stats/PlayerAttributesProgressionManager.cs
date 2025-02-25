using Gameplay.Character;
using Gameplay.Character.Attributes;
using Gameplay.Character.Attributes.Dexterity;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Player.Stats
{
    public static class PlayerAttributesProgressionManagerDebug
    {
        [MenuItem("Debug/Reset Attributes Progression")]
        public static void ResetAttributesProgression()
        {
            PlayerDexterityManager.ResetPlayerDexterity();
            PlayerEnduranceManager.ResetPlayerEndurance();
        }
    }

    public class PlayerAttributesProgressionManager : MonoBehaviour
    {
        public CharacterStatProfile characterStatProfile;

        public PlayerDexterityManager playerDexterityManager;
        public PlayerEnduranceManager playerEnduranceManager;


        void InitializeAttributes()
        {
            PlayerDexterityManager.Initialize(characterStatProfile);
            PlayerEnduranceManager.Initialize(characterStatProfile);
        }
    }
}
