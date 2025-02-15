using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Core.SaveSystem
{
    public class SaveStateManager : MonoBehaviour, MMEventListener<CheckPointEvent>
    {
        [Tooltip("Is a valid save loaded?")] public bool IsSaveLoaded;


        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(CheckPointEvent mmEvent)
        {
            MMGameEvent.Trigger("SaveResources");
            MMGameEvent.Trigger("RevertResources");
            MMGameEvent.Trigger("SaveJournal");
            MMGameEvent.Trigger("RevertJournal");
        }
    }
}
