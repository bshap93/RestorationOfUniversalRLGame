using MoreMountains.Feedbacks;
using UnityEngine;

namespace Project.UI.Popups
{
    public class FloatingTextAddon : MonoBehaviour
    {
        // Subscribe to the MMFloatingTextSpawnEvent
        void OnEnable()
        {
            MMFloatingTextSpawnEvent.Register(OnFloatingTextSpawned);
        }

        // Unsubscribe when disabled
        void OnDisable()
        {
            MMFloatingTextSpawnEvent.Unregister(OnFloatingTextSpawned);
        }

        // Event handler for floating text
        void OnFloatingTextSpawned(
            MMChannelData channelData, Vector3 spawnPosition, string value, Vector3 direction, float intensity,
            bool forceLifetime = false, float lifetime = 1f, bool forceColor = false,
            Gradient animateColorGradient = null,
            bool useUnscaledTime = false)
        {
            // Modify the value to truncate or round the number
            if (float.TryParse(value, out var numericValue))
                value = Mathf.Round(numericValue).ToString(); // Round to nearest integer

            // Or truncate: value = Mathf.FloorToInt(numericValue).ToString();
            // Trigger the modified event
            MMFloatingTextSpawnEvent.Trigger(
                channelData, spawnPosition, value, direction, intensity,
                forceLifetime, lifetime, forceColor, animateColorGradient, useUnscaledTime);
        }
    }
}
