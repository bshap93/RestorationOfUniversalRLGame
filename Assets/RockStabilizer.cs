using UnityEngine;

public class RockStabilizer : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Prevent the rock from entering "sleep" state
        rb.sleepThreshold = 0;
    }
}
