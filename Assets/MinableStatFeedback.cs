using Core.Events;
using UnityEngine;

public class MinableStatFeedback : MonoBehaviour
{
    public float enduranceExperiencePoints;

    public void DispatchEvent()
    {
        AttributeEvent.Trigger(
            AttributeInQuestion.Endurance, AttributeEventType.IncreaseExperiencePoints, enduranceExperiencePoints);
    }
}
