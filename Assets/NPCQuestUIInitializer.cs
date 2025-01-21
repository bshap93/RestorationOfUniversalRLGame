using PixelCrushers.QuestMachine;
using UnityEngine;

public class NpcQuestUIInitializer : MonoBehaviour
{
    QuestGiver questGiver;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("Starting NPCQuestUIInitializer");
        questGiver = GetComponent<QuestGiver>();
        var questDialogueUI = FindObjectOfType<UnityUIQuestDialogueUI>();

        if (questDialogueUI != null)
            questGiver.questDialogueUI = questDialogueUI;

        else if (questDialogueUI == null)
            Debug.LogWarning(
                "Quest Machine: Can't find UnityUIQuestDialogueUI in scene. Make sure it's in the scene before the NPCQuestUIInitializer.");
    }

    // Update is called once per frame
    void Update()
    {
    }
}
