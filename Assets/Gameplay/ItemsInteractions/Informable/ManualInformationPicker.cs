using Gameplay.ItemsInteractions.Informable;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ManualInformationPicker : MonoBehaviour
{
    public InformableItem Informable;
    public MMFeedbacks InformationFeedbacks; // Feedbacks to play when the information is shown

    bool _isInRange;

    void Update()
    {
        if (_isInRange && Input.GetKeyDown(KeyCode.F)) ShowInformation();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) _isInRange = true;
        // Show UI prompt to press 'F' to read
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) _isInRange = false;
        // Hide UI prompt
    }

    void ShowInformation()
    {
        // Activate UI to show the information
        InformationFeedbacks?.PlayFeedbacks();
        Debug.Log($"Showing information: {Informable.InformationTitle}");
        // Display Informable.InformationDescription on the screen
    }
}
