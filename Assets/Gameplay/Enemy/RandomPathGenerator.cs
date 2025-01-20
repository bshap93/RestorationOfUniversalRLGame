using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;
using UnityEngine.AI;

namespace Project.Gameplay.Enemy
{
    public class RandomPathGenerator : MonoBehaviour
    {
        public int numberOfPoints = 5; // Number of points in the path
        public float radius = 10f; // Radius to search for points around the spawn
        public float pointHeight = 1f; // Fixed Y position for the points
        public float delayBetweenPoints; // Delay for each point in the path
        public MMPath.CycleOptions cycleOption = MMPath.CycleOptions.Loop; // Path cycle option
        public GameObject enemyPrefab; // Enemy prefab with an MMPath component

        /// <summary>
        ///     Generates a random path for an enemy and spawns it at the given position.
        /// </summary>
        /// <param name="spawnPosition">The position where the enemy is spawned</param>
        public List<MMPathMovementElement> GenerateRandomPath(Vector3 spawnPosition)
        {
            // List to store the path elements
            var pathElements = new List<MMPathMovementElement>();

            // Generate random points on the NavMesh
            for (var i = 0; i < numberOfPoints; i++)
            {
                var randomDirection = Random.insideUnitSphere * radius;
                randomDirection += spawnPosition;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
                {
                    var point = hit.position;
                    point.y = spawnPosition.y; // Fix Y position to the spawn height

                    pathElements.Add(
                        new MMPathMovementElement
                        {
                            PathElementPosition = point - spawnPosition, // Relative position for MMPath
                            Delay = delayBetweenPoints
                        });
                }
            }

            if (pathElements.Count < 2)
            {
                Debug.LogWarning("Not enough valid path points found on the NavMesh.");
                return null; // No valid path was found
            }

            Debug.Log("Path generated with " + pathElements.Count + " points.");
            return pathElements;
        }
    }
}
