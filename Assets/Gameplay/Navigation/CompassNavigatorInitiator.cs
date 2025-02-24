using CompassNavigatorPro;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Gameplay.Navigation
{
    public class CompassNavigatorInitiator : MonoBehaviour, MMEventListener<MMCameraEvent>
    {
        CompassPro _compassPro;

        void Start()
        {
            _compassPro = GetComponent<CompassPro>();
        }
        void OnEnable()
        {
            // Start listening for both MMGameEvent and MMCameraEvent
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            // Stop listening to avoid memory leaks
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMCameraEvent mmEvent)
        {
            if (mmEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
            {
                if (_compassPro == null)
                    _compassPro = FindFirstObjectByType<CompassPro>();

                if (_compassPro != null) _compassPro.follow = mmEvent.TargetCharacter.transform;
            }
        }
    }
}
