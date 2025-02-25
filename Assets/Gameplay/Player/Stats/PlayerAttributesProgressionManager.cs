using Gameplay.Character.Attributes;
using UnityEditor;
using UnityEngine;

namespace Gameplay.Player.Stats
{
    public static class PlayerAttributesProgressionManagerDebug
    {
        [MenuItem("Debug/Reset Attributes Progression")]
        public static void ResetAttributesProgression()
        {
            
        }
    }
    public class PlayerAttributesProgressionManager : MonoBehaviour
    {
        public static void ResetPlayerAttributesProgression()
        {
            PlayerDexterityManager.ResetPlayerDexterity();
        }
    }
}
