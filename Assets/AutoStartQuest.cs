using PixelCrushers.QuestMachine;
using UnityEngine;

public class AutoStartQuest : MonoBehaviour
{
    public Quest questToStart; // Assign this via the Inspector

    void Start()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var questListContainer = GetComponent<QuestListContainer>();
        if (questListContainer != null && questToStart != null)
        {
            var questInstance = questListContainer.AddQuest(questToStart);
            if (questInstance != null)
            {
                questInstance.SetState(QuestState.Active);
                QuestMachineMessages.RefreshUIs(null);
                Debug.Log("Auto-started quest: " + questToStart.name);
            }
        }
    }
}
