using PixelCrushers;
using UnityEngine;

public class CheckpointQuestObjective : MonoBehaviour
{
    [Tooltip("Message to send when this checkpoint is triggered.")]
    public string message = "CheckpointReached";

    [Tooltip("Message value to send with the message (optional).")]
    public string messageValue = "";

    void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to the player or relevant quest actor.
        if (other.CompareTag("Player")) // Ensure the Player tag is set on the player GameObject.
        {
            // Send a message to Quest Machine to progress the quest.
            MessageSystem.SendMessage(this, message, messageValue);
            Debug.Log($"Message sent: {message} with value: {messageValue}");

            // Optional: Disable the trigger to prevent multiple activations.
            // gameObject.SetActive(false);
        }
    }
}
