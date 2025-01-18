using MoreMountains.Feedbacks;
using UnityEngine;

[AddComponentMenu("Feedbacks/Custom/Disable Rigidbody")]
public class MMFeedbackDisableRigidbody : MMFeedback
{
    [Header("Rigidbody Settings")] public Rigidbody TargetRigidbody; // Assign the Rigidbody to disable

    protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
    {
        if (Active && TargetRigidbody != null)
        {
            // Disable the Rigidbody
            TargetRigidbody.linearVelocity = Vector3.zero; // Stop motion
            TargetRigidbody.isKinematic = true; // Disable physics
        }
    }
}
