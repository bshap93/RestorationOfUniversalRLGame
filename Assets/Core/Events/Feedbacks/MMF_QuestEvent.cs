using MoreMountains.Feedbacks;
using PixelCrushers.QuestMachine.Wrappers;
using UnityEngine;

namespace Core.Events.Feedbacks
{
    /// <summary>
    ///     This feedback lets you trigger TopDown Engine Events, that can then be caught by other classes
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Events/Quest Events")]
    [FeedbackHelp("This feedback lets you trigger Quest Events, that can then be caught by other classes")]
    public class MMF_QuestEvent : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;

        [MMFInspectorGroup("Quest Events", true, 17)] [Tooltip("the type of event to trigger")]
        public QuestEventType EventType = QuestEventType.ForcePlayerToAcceptQuest;
        public Quest QuestTarget;
        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor
        {
            get { return MMFeedbacksInspectorColors.EventsColor; }
        }

#endif
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (Active) MMQuestEvent.Trigger(EventType, QuestTarget);
        }
    }
}
