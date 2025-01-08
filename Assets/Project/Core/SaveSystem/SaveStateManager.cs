using MoreMountains.Tools;
using TopDownEngine.Common.Scripts.Spawn;
using UnityEngine;

namespace Project.Core.SaveSystem
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

        public void OnMMEvent(CheckPointEvent checkPointEvent)
        {
            Debug.Log("SaveStateManager: Checkpoint reached.");
            MMGameEvent.Trigger("RevertResources");
            MMGameEvent.Trigger("RevertJournal");
        }
    }
}
