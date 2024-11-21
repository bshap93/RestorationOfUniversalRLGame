using MoreMountains.Feedbacks;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class MagicResource : MonoBehaviour
    {
        public string ResourceName; // Name of the resource (e.g., Kinema, Favour)
        public float MaxResource = 100f; // Maximum resource capacity
        public float CurrentResource; // Current resource amount
        public float RegenerationRate = 5f; // Amount regenerated per second

        public MMFeedbacks OnDepletedFeedback; // Feedbacks when the resource is depleted
        public MMFeedbacks OnRestoredFeedback; // Feedbacks when the resource is restored


        void Start()
        {
            CurrentResource = MaxResource;
        }

        void Update()
        {
            RegenerateResource();
        }

        public void ConsumeResource(float amount)
        {
            CurrentResource = Mathf.Max(CurrentResource - amount, 0f);

            if (CurrentResource <= 0f) OnResourceDepleted();
        }

        public void RestoreResource(float amount)
        {
            CurrentResource = Mathf.Min(CurrentResource + amount, MaxResource);

            if (amount > 0) OnResourceRestored();
        }

        protected virtual void OnResourceDepleted()
        {
            Debug.Log($"{ResourceName} depleted.");
            OnDepletedFeedback?.PlayFeedbacks();
        }

        protected virtual void OnResourceRestored()
        {
            Debug.Log($"{ResourceName} restored.");
            OnRestoredFeedback?.PlayFeedbacks();
        }

        void RegenerateResource()
        {
            if (CurrentResource < MaxResource) RestoreResource(RegenerationRate * Time.deltaTime);
        }
    }
}
