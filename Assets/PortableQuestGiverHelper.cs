using System.Collections.Generic;
using Core.Events;
using MoreMountains.Tools;
using PixelCrushers.QuestMachine;
using UnityEngine;

public class PortableQuestGiverHelper : MonoBehaviour, MMEventListener<MMQuestEvent>
{
    QuestGiver _questGiver;
    List<Quest> _quests = new();

    void Awake()
    {
        _questGiver = GetComponent<QuestGiver>();
        if (_questGiver == null) Debug.LogError("QuestGiver component not found on " + gameObject.name);

        _quests = _questGiver.questList;
    }

    void OnEnable()
    {
        this.MMEventStartListening();
    }

    void OnDisable()
    {
        this.MMEventStopListening();
    }

    public void OnMMEvent(MMQuestEvent mmEvent)
    {
        switch (mmEvent.EventType)
        {
            case QuestEventType.ForcePlayerToAcceptQuest:
                Quest quest = mmEvent.QuestParameter;
                if (_quests.Contains(quest))
                {
                    Debug.Log("Forcing player to accept quest: " + quest.name);
                    _questGiver.OnAcceptQuest(quest);
                }

                break;
        }
    }
}
