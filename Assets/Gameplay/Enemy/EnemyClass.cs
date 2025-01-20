using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Project.Gameplay.Enemy
{
    [Serializable]
    public class EnemyClass
    {
        public string EnemyClassID;

        public List<GameObject> EnemyPrefabs;

        public GameObject GetRandomEnemyPrefab()
        {
            return EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)];
        }
    }
}
