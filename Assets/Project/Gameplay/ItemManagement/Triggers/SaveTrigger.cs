using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class SaveTrigger : MonoBehaviour
    {
        public static void Save()
        {
            Debug.LogWarning("=== SaveTrigger.Save() called ===");
            MMGameEvent.Trigger("SaveInventory");
            MMGameEvent.Trigger("SaveResources");
        }
    }
}
