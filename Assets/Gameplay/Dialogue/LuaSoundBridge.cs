using MoreMountains.Tools;
using PixelCrushers.DialogueSystem;
using UnityEngine;

namespace Gameplay.Dialogue
{
    public class LuaSoundBridge : MonoBehaviour
    {
        void Start()
        {
            // Register Lua function to play sounds
            Lua.RegisterFunction("PlayMMSound", this, SymbolExtensions.GetMethodInfo(() => PlayMMSound("")));
        }

        public static void PlayMMSound(string soundName)
        {
            AudioClip clip = Resources.Load<AudioClip>(soundName); // Ensure your sound is in Resources
            if (clip != null && MMSoundManager.Instance != null)
            {
                MMSoundManager.Instance.PlaySound(clip, MMSoundManager.MMSoundManagerTracks.Sfx, Vector3.zero);
            }
            else
            {
                Debug.LogWarning($"Sound {soundName} not found or MMSoundManager not initialized!");
            }
        }
    }
}
