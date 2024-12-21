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
        // Sync NavMeshAgent movement with CharacterMovement
        if (_agent.enabled && _agent.hasPath)
        {
            var moveDirection = _agent.desiredVelocity.normalized;

            // Send movement data to CharacterMovement
            _characterMovement.SetHorizontalMovement(moveDirection.x);
            _characterMovement.SetVerticalMovement(moveDirection.z);
        }
        else
        {
            // Stop character when NavMeshAgent has no path
            _characterMovement.SetHorizontalMovement(0);
            _characterMovement.SetVerticalMovement(0);
        }
    }

    public void SetDestination(Vector3 destination)
    {
        if (_agent.enabled) _agent.SetDestination(destination);
    }
}
