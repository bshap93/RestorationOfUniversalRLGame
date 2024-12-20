using DunGen;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Gameplay.Navigation;
using UnityEngine;

public class GeneratorInitiator : MonoBehaviour, MMEventListener<MMCameraEvent>
{
    StaticMapRoomCulling _adjacentRoomCulling;

    void Start()
    {
        _adjacentRoomCulling = GetComponent<StaticMapRoomCulling>();
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

    public void OnMMEvent(MMCameraEvent eventType)
    {
        if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
        {
            if (_adjacentRoomCulling == null)
                _adjacentRoomCulling = FindObjectOfType<StaticMapRoomCulling>();

            if (_adjacentRoomCulling != null) _adjacentRoomCulling.TargetOverride = eventType.TargetCharacter.transform;
        }
    }
}
