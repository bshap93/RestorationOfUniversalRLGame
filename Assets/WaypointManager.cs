using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class WaypointManager : MonoBehaviour
{
    public BehaviorTree behaviorTree; // Reference to the Behavior Tree
    public List<Vector3> waypoints; // List of raw Vector3 positions

    void Start()
    {
        var transforms = new List<GameObject>();

        foreach (var waypoint in waypoints)
        {
            var tempWaypoint = new GameObject("Waypoint");
            tempWaypoint.transform.position = waypoint;
            transforms.Add(tempWaypoint);
        }

        // Set the Transform List in the Behavior Designer's Patrol task
        behaviorTree.SetVariableValue("Waypoints", transforms);
    }
}
