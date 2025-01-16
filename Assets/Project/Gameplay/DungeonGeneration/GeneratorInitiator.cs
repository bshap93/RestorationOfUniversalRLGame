using DunGen;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class GeneratorInitiator : MonoBehaviour, MMEventListener<MMCameraEvent>
{
    AdjacentRoomCulling _adjacentRoomCulling;

    void Start()
    {
        _adjacentRoomCulling = GetComponent<AdjacentRoomCulling>();
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

    public void OnMMEvent(MMCameraEvent @event)
    {
        if (@event.EventType == MMCameraEventTypes.SetTargetCharacter)
        {
            if (_adjacentRoomCulling == null)
                _adjacentRoomCulling = FindObjectOfType<AdjacentRoomCulling>();

            if (_adjacentRoomCulling != null)
                _adjacentRoomCulling.TargetOverride = @event.TargetCharacter.transform;
        }
    }
}
