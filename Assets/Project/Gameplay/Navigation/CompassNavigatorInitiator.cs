using CompassNavigatorPro;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

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

    public void OnMMEvent(MMCameraEvent cookingStationEvent)
    {
        if (cookingStationEvent.EventType == MMCameraEventTypes.SetTargetCharacter)
        {
            if (_compassPro == null)
                _compassPro = FindObjectOfType<CompassPro>();

            if (_compassPro != null) _compassPro.follow = cookingStationEvent.TargetCharacter.transform;
        }
    }
}
