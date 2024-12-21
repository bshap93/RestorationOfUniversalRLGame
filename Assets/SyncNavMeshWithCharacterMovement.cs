using Project.Gameplay.AI;
using UnityEngine;
using UnityEngine.AI;

public class SyncNavMeshWithCharacterMovement : MonoBehaviour
{
    CustomCharacterMovement _characterMovement;
    NavMeshAgent _navMeshAgent;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _characterMovement = GetComponent<CustomCharacterMovement>();
    }

    void Update()
    {
        // Get the velocity of the NavMeshAgent
        var velocity = _navMeshAgent.velocity;

        // Calculate movement input (retain speed for realistic movement)
        var movementInput = new Vector2(velocity.x, velocity.z);

        // Smooth movement to avoid jitter
        var smoothInput = Vector2.Lerp(_characterMovement.GetCurrentInput(), movementInput, Time.deltaTime * 10f);

        // Stop movement if NavMeshAgent is close to the target
        if (_navMeshAgent.remainingDistance < _navMeshAgent.stoppingDistance && !_navMeshAgent.pathPending)
            smoothInput = Vector2.zero;

        // Update the CharacterMovement
        _characterMovement.SetMovement(smoothInput);

        // Handle rotation synchronization
        if (_navMeshAgent.updateRotation == false) RotateTowardsVelocity(velocity);
    }

    void RotateTowardsVelocity(Vector3 velocity)
    {
        if (velocity.sqrMagnitude > 0.01f)
        {
            var targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }
}
