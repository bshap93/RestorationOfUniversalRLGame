using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.AI;

public class SyncNavMeshWithCharacterMovement : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    private CharacterMovement _characterMovement;
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _characterMovement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        // Get the velocity of the NavMeshAgent
        Vector3 velocity = _navMeshAgent.velocity;

        // Calculate normalized movement direction (like CharacterMovement expects)
        Vector2 normalizedMovement = new Vector2(velocity.x, velocity.z).normalized;

        // Set the movement of CharacterMovement (it expects x and y, not z)
        _characterMovement.SetMovement(normalizedMovement);
    }
}
