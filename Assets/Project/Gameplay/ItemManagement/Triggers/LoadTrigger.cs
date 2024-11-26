using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class LoadTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        public static void Revert()
        {
            MMGameEvent.Trigger("RevertResources");
        }
    }
}
