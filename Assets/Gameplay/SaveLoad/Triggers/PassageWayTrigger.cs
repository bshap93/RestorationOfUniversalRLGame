using MoreMountains.Tools;
using PixelCrushers;
using UnityEngine;

namespace Gameplay.SaveLoad.Triggers
{
    public class PassageWayTrigger : MonoBehaviour
    {
        public string parameter;
        public static void Save()
        {
            MMGameEvent.Trigger("SaveInventory");
            MMGameEvent.Trigger("SaveResources");
            MMGameEvent.Trigger("SaveJournal");
        }

        public void SendQuestMessage()
        {
            MessageSystem.SendMessage(this, "PassageWayEntered", parameter);
        }
    }
}
