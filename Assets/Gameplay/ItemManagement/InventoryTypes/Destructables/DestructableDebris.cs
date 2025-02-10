using System.Collections;
using MoreMountains.Feedbacks;
using UnityEngine;

public class DestructibleRock : MonoBehaviour
{
    [Header("Settings")] [SerializeField] float minimumVelocity = 0.1f;
    [SerializeField] float destructionDelay = 0.5f;
    [SerializeField] float minimumMovementTime = 1.5f; // Minimum time rock must be moving
    [SerializeField] float stoppedTime = 1f; // How long it must be stopped before destroying

    [Header("Feedbacks")] [SerializeField] MMFeedbacks destructionFeedback;
    bool hasBeenPushed;
    bool isDestroying;

    Rigidbody rb;
    float timeMoving;
    float timeStoppedFor;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (isDestroying) return;

        // Check if rock is moving
        if (rb.linearVelocity.magnitude > minimumVelocity)
        {
            hasBeenPushed = true;
            timeMoving += Time.deltaTime;
            timeStoppedFor = 0f; // Reset stopped timer while moving
        }
        // If rock has been pushed and is now stopped
        else if (hasBeenPushed)
        {
            timeStoppedFor += Time.deltaTime;
        }

        // Only destroy if:
        // 1. Rock has moved for minimum time
        // 2. Has been stopped for required time
        if (timeMoving >= minimumMovementTime && timeStoppedFor >= stoppedTime)
        {
            isDestroying = true;
            StartCoroutine(DestroyAfterDelay());
        }
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(destructionDelay);

        if (destructionFeedback != null) destructionFeedback.PlayFeedbacks();

        yield return new WaitForSeconds(0.1f);

        Destroy(gameObject);
    }
}
