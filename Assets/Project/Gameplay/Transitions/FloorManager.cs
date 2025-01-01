using System.Collections.Generic;
using UnityEngine;

namespace Project.Gameplay.Transitions
{
    public class FloorManager : MonoBehaviour
    {
        [Header("Visibility Settings")] [Range(0f, 1f)]
        public float upperFloorAlpha = 0.3f;
        [Range(0f, 1f)] public float lowerFloorAlpha = 1f;

        [Header("Dynamic Visibility")] public bool useDynamicVisibility = true;
        public float visibilityRadius = 5f;
        public float fadeSpeed = 5f;

        readonly List<Floor> floors = new();
        Transform player;
        public static FloorManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Find all floors in the scene
            var foundFloors = FindObjectsOfType<Floor>();
            floors.AddRange(foundFloors);

            // Find player
            player = FindObjectOfType<FloorTriggerDetector>().transform;
        }

        void Update()
        {
            if (useDynamicVisibility) UpdateDynamicVisibility();
        }

        public void UpdateFloorVisibility(Floor currentFloor)
        {
            foreach (var floor in floors)
                if (floor.floorLevel > currentFloor.floorLevel)
                    floor.SetTransparency(upperFloorAlpha);
                else
                    floor.SetTransparency(lowerFloorAlpha);
        }

        void UpdateDynamicVisibility()
        {
            foreach (var floor in floors)
                if (floor.floorLevel > player.GetComponent<FloorTriggerDetector>().CurrentFloor.floorLevel)
                {
                    // Calculate distance-based alpha
                    var distanceToPlayer = Vector3.Distance(
                        new Vector3(player.position.x, 0, player.position.z),
                        new Vector3(floor.transform.position.x, 0, floor.transform.position.z)
                    );

                    var targetAlpha = Mathf.Lerp(
                        0.1f, upperFloorAlpha,
                        Mathf.Clamp01(distanceToPlayer / visibilityRadius));

                    floor.SetTransparency(targetAlpha);
                }
        }

        // Optional: Manual floor toggling
        public void ToggleFloorVisibility(int floorLevel, bool visible)
        {
            foreach (var floor in floors)
                if (floor.floorLevel == floorLevel)
                    floor.SetTransparency(visible ? lowerFloorAlpha : upperFloorAlpha);
        }
    }
}
