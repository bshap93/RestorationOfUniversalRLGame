using UnityEngine;
using MoreMountains.Feedbacks;

public class InvisibleWallMmFeedback : MonoBehaviour
{
    public MMFeedbacks feedbacks;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            feedbacks?.PlayFeedbacks();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            feedbacks?.PlayFeedbacks();
        }
    }
}
