using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimplifiedNavMeshController : MonoBehaviour
{
    public float rotationSpeed = 5f;
    CharacterController _characterController;
    NavMeshAgent _navMeshAgent;
    Transform _transform;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _characterController = GetComponent<CharacterController>();
        _transform = transform;

        // Ensure NavMeshAgent doesn't control rotation directly
        _navMeshAgent.updateRotation = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    void HandleMovement()
    {
        // Update CharacterController based on NavMeshAgent's movement
        if (_characterController)
        {
            var movement = _navMeshAgent.velocity * Time.deltaTime;
            _characterController.Move(movement);
        }
    }

    void HandleRotation()
    {
        // Rotate towards movement direction smoothly
        var velocity = _navMeshAgent.velocity;
        if (velocity.sqrMagnitude > 0.01f)
        {
            var targetRotation = Quaternion.LookRotation(velocity.normalized, Vector3.up);
            _transform.rotation = Quaternion.Slerp(_transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
