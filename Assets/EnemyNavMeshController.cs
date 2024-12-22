using MoreMountains.TopDownEngine;
using TopDownEngine.Common.Scripts.Characters.CharacterAbilities;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyNavMeshController : MonoBehaviour
{
    NavMeshAgent _agent;
    Character _character;
    CharacterMovement _characterMovement;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _character = GetComponent<Character>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        // If the NavMeshAgent is not enabled or has no path, stop the character
        if (!_agent.enabled || !_agent.hasPath)
        {
            StopCharacterMovement();
            return;
        }

        // Check if the agent is close enough to the destination
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance)
        {
            StopCharacterMovement();
        }
        else
        {
            // Move the character based on NavMeshAgent's desired velocity
            var moveDirection = _agent.desiredVelocity.normalized;
            _characterMovement.SetHorizontalMovement(moveDirection.x);
            _characterMovement.SetVerticalMovement(moveDirection.z);
        }
    }

    public void SetDestination(Vector3 destination)
    {
        if (_agent.enabled) _agent.SetDestination(destination);
    }

    void StopCharacterMovement()
    {
        _characterMovement.SetHorizontalMovement(0);
        _characterMovement.SetVerticalMovement(0);
    }
}
