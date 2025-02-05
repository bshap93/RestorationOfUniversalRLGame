using MoreMountains.Feedbacks;
using PixelCrushers.QuestMachine;
using UnityEngine;

namespace Core.Events.EventTriggers
{
    public class LevelStartEvents : MonoBehaviour
    {
        public MMFeedbacks LevelStartFeedbacks;
        // Start is called once before the first execution of Update after the MonoBehaviour is created

        public QuestEvent QuestEvent;

        void Awake()
        {
            Debug.Log("LevelStartEvents Awake");
            LevelStartFeedbacks?.PlayFeedbacks();
        }
    }
}
