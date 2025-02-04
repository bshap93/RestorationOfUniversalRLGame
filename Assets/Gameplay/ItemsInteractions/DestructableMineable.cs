using Gameplay.ItemManagement.InventoryTypes.Destructables;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class DestructableMineable : MonoBehaviour
    {
        public string UniqueID;
        public Destructable destructable;
        GameObject _currentInstance;
        int _currentPrefabIndex = -1;
        Health _health;

        void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
                Debug.LogWarning("UniqueID is not set for DestructableMineable");

            _health = GetComponent<Health>();
            if (_health != null)
            {
                _health.OnHit += HandleHit;
                _health.OnDeath += HandleDeath;
            }

            _health.MaximumHealth = destructable.maxHealth;
            InitializeState();
        }

        void InitializeState()
        {
            if (destructable == null) return;

            // Start with the intact prefab
            _currentInstance = Instantiate(
                destructable.prefabIntact, transform.position, transform.rotation, transform);

            _currentPrefabIndex = 0;
        }

        void HandleHit()
        {
            if (destructable == null || _health == null) return;

            var healthPercentage = _health.CurrentHealth / destructable.maxHealth;
            var newPrefabIndex = GetPrefabIndex(healthPercentage);

            if (newPrefabIndex != _currentPrefabIndex)
                UpdatePrefab(newPrefabIndex);
        }

        void HandleDeath()
        {
            if (destructable == null) return;

            // Remove intact instance
            Destroy(_currentInstance);

            // Choose a random destroyed prefab
            if (destructable.destroyedPrefabs != null && destructable.destroyedPrefabs.Count > 0)
            {
                var destroyedPrefab = destructable.destroyedPrefabs[
                    Random.Range(0, destructable.destroyedPrefabs.Count)
                ];

                Instantiate(destroyedPrefab, transform.position, transform.rotation);
            }

            // Spawn loot separately
            SpawnLoot();

            // Play destruction feedback
            _health.DeathMMFeedbacks?.PlayFeedbacks(transform.position);

            // Remove the object from the scene
            Destroy(gameObject);
        }

        void SpawnLoot()
        {
            if (destructable.possibleLoot == null || destructable.possibleLoot.Count == 0) return;

            var dropAmount = Random.Range(destructable.dropAmountRange.x, destructable.dropAmountRange.y + 1);

            for (var i = 0; i < dropAmount; i++)
            {
                // Pick a random loot prefab from the list
                var lootToSpawn = destructable.possibleLoot[
                    Random.Range(0, destructable.possibleLoot.Count)
                ];

                if (lootToSpawn != null)
                    Instantiate(lootToSpawn, transform.position + Random.insideUnitSphere * 0.5f, Quaternion.identity);
            }
        }

        int GetPrefabIndex(float healthPercentage)
        {
            for (var i = 0; i < destructable.intermediateHealthThresholds.Count; i++)
                if (healthPercentage > destructable.intermediateHealthThresholds[i])
                    return i;

            return destructable.intermediatePrefabs.Count;
        }

        void UpdatePrefab(int newPrefabIndex)
        {
            if (_currentInstance != null)
                Destroy(_currentInstance);

            if (newPrefabIndex < destructable.intermediatePrefabs.Count)
                _currentInstance = Instantiate(
                    destructable.intermediatePrefabs[newPrefabIndex], transform.position, transform.rotation,
                    transform);
            else
                _currentInstance = Instantiate(
                    destructable.destroyedPrefabs[0], transform.position, transform.rotation, transform);

            _currentPrefabIndex = newPrefabIndex;
        }
    }
}
