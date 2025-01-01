using UnityEngine;

// Attach this to the player
namespace Project.Gameplay.Transitions
{
    public class FloorTriggerDetector : MonoBehaviour
    {
        public Floor CurrentFloor { get; private set; }

        void OnTriggerEnter(Collider other)
        {
            var floor = other.GetComponent<Floor>();
            if (floor != null)
            {
                CurrentFloor = floor;
                FloorManager.Instance.UpdateFloorVisibility(CurrentFloor);
            }
        }
    }
}
