using Pathfinding;
using TopDownEngine.Common.Scripts.Characters.CharacterAbilities;
using UnityEngine;

public class AStarPathfinderController : MonoBehaviour
{
    public Transform target; // The target to move towards
    public float nextWaypointDistance = 0.5f; // Distance to switch to the next waypoint
    CharacterMovement _characterMovement;
    int _currentWaypointIndex;
    Path _path;
    Seeker _seeker;

    void Start()
    {
        _seeker = GetComponent<Seeker>();
        _characterMovement = GetComponent<CharacterMovement>();

        if (target != null) FindPath();
    }

    void Update()
    {
        if (_path == null || _currentWaypointIndex >= _path.vectorPath.Count)
        {
            StopCharacterMovement();
            return;
        }

        // Check if close enough to the current waypoint
        var distanceToWaypoint = Vector3.Distance(transform.position, _path.vectorPath[_currentWaypointIndex]);
        if (distanceToWaypoint < nextWaypointDistance)
        {
            _currentWaypointIndex++;
            return;
        }

        // Move towards the current waypoint
        var direction = (_path.vectorPath[_currentWaypointIndex] - transform.position).normalized;
        _characterMovement.SetHorizontalMovement(direction.x);
        _characterMovement.SetVerticalMovement(direction.z);
    }

    void LateUpdate()
    {
        // If the target moves, recalculate the path periodically
        if (target != null) FindPath();
    }

    void FindPath()
    {
        // Request a path to the target
        _seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypointIndex = 0;
        }
    }

    void StopCharacterMovement()
    {
        _characterMovement.SetHorizontalMovement(0);
        _characterMovement.SetVerticalMovement(0);
    }
}
