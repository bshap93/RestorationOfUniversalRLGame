using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class SaveTrigger : MonoBehaviour
    {
        public static void Save()
        {
            MMGameEvent.Trigger("SaveInventory");
        }
        
    }
}
