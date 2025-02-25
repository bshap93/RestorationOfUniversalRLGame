using Gameplay.Character.Attributes.Dexterity;
using Gameplay.Character.Attributes.Endurance;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.Character.Attributes
{
    public static class PlayerAttributesProgressionManagerDebug
    {
        [MenuItem("Debug/Reset Attributes Progression")]
        public static void ResetAttributesProgression()
        {
            PlayerDexterityManager.ResetPlayerDexterity();
            SimplifiedEnduranceManager.ResetPlayerEndurance();
        }
    }

    public class PlayerAttributesProgressionManager : MonoBehaviour
    {
        public SimplifiedDexterityManager playerDexterityManager;
        [FormerlySerializedAs("playerEnduranceManager")]
        public SimplifiedEnduranceManager simplifiedEnduranceManager;


        public void Reset()
        {
            var charStateProf =
                Resources.Load<CharacterStatProfile>(CharacterResourcePaths.CharacterStatProfileFilePath);

            SimplifiedDexterityManager.ResetPlayerDexterity();
            SimplifiedEnduranceManager.ResetPlayerEndurance();
        }

        public static void ResetPlayerAttributesProgression()
        {
            SimplifiedDexterityManager.ResetPlayerDexterity();
            SimplifiedEnduranceManager.ResetPlayerEndurance();
        }


        public static CharacterStatProfile GetCharacterStatProfile()
        {
            return Resources.Load<CharacterStatProfile>(
                CharacterResourcePaths.CharacterStatProfileFilePath);
        }
    }
}
