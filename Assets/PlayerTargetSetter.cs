using BehaviorDesigner.Runtime;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using VTabs.Libs;

public class PlayerTargetSetter : MonoBehaviour, MMEventListener<MMCameraEvent>
{
    BehaviorTree _behaviorTree;
    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMCameraEvent eventType)
    {
        if (eventType.EventType == MMCameraEventTypes.SetTargetCharacter)
        {
            _behaviorTree = GetComponent<BehaviorTree>();
            if (_behaviorTree != null) _behaviorTree.SetPropertyValue("PlayerTarget", eventType.TargetCharacter);
        }
    }
}
