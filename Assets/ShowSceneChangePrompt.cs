using Project.UI.HUD;
using UnityEngine;
using UnityEngine.Serialization;

public class ShowSceneChangePrompt : MonoBehaviour
{
    [FormerlySerializedAs("prompt")] public EnterScenePromptManager promptManager;

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player")) promptManager.ShowChangeScnenePrompt();
        Debug.Log("Player entered the trigger");
    }
}
