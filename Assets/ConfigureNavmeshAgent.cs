using UnityEngine;
using UnityEngine.AI;

public class ConfigureNavMeshAgent : MonoBehaviour
{
    NavMeshAgent _agent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();

        // Disable automatic position and rotation updates
        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }
}
