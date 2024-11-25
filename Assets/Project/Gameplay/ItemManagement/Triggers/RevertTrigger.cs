using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.Triggers
{
    public class RevertTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        public static void Revert()
        {
            MMGameEvent.Trigger("Revert");
        }
    }
}
