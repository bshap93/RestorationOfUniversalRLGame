using System.Collections.Generic;
using Project.Core.SaveSystem;
using UnityEngine;

namespace Project.Gameplay.Transitions
{
    public class FloorManager : MonoBehaviour
    {
        [Header("Floor Setup")] public List<GameObject> floors; // List of all floors

        public List<Collider> floorColliders; // List of all floor colliders
        public SpawnPoint initialSpawnPoint;
        public Dictionary<string, Collider> FloorColliderDictionary = new();
        public Dictionary<string, GameObject> FloorDictionary = new();

        void Start()
        {
            // Initialize the floor dictionary
            foreach (var floor in floors)
            {
                var floorComponent = floor.GetComponent<FloorLevel>();
                if (floorComponent != null)
                {
                    FloorDictionary[floorComponent.floorName] = floor;
                    FloorColliderDictionary[floorComponent.floorName] = floorColliders[floors.IndexOf(floor)];
                }
                else
                {
                    Debug.LogWarning($"Floor {floor.name} is missing a FloorID component.");
                }
            }

            // Set the initial floor
            if (initialSpawnPoint != null)
                SetFloorVisibility(initialSpawnPoint.FloorLevel.floorName);
        }
        public void SetFloorVisibility(string floorName)
        {
            Debug.Log("Setting floor visibility to " + floorName);
            foreach (var kvp in
                     FloorDictionary)
                kvp.Value.SetActive(kvp.Key == floorName); // Enable the matching floor, disable others
        }
    }
}
