using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.SaveLoad.Triggers
{
    public class RevertTrigger : MonoBehaviour
    {
        // Start is called before the first frame update
        public static void Revert()
        {
            MMGameEvent.Trigger("RevertInventory");
            MMGameEvent.Trigger("RevertResources");
            MMGameEvent.Trigger("RevertJournal");
        }
    }
}
