using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class DestructableMineable : BaseDestructable
    {
        GameObject _currentInstance;
        int _currentPrefabIndex = -1;

        protected override void Awake()
        {
            base.Awake();


            if (Health != null)
            {
                Health.OnHit += HandleHit;
                Health.OnDeath += HandleDeath;
            }

            if (Health != null) Health.MaximumHealth = destructable.maxHealth;
            InitializeState();
        }

        void InitializeState()
        {
            if (destructable == null) return;

            // Start with the intact prefab
            _currentInstance = gameObject;

            _currentPrefabIndex = 0;
        }

        void HandleHit()
        {
            if (destructable == null || Health == null) return;

            var healthPercentage = Health.CurrentHealth / destructable.maxHealth;
            var newPrefabIndex = GetPrefabIndex(healthPercentage);

            if (newPrefabIndex != _currentPrefabIndex)
                UpdatePrefab(newPrefabIndex);
        }

        void HandleDeath()
        {
            if (destructable == null) return;

            if (destructable == null) Debug.LogWarning("No destructable ScriptableObject found on " + gameObject.name);
            IsBeingDestroyed = true;

            // Save the destroyed state
            DestructableManager.SaveDestroyedContainer(UniqueID);


            // Choose a random destroyed prefab
            if (destructable.destroyedPrefabs != null && destructable.destroyedPrefabs.Count > 0)
            {
                var destroyedPrefab = destructable.destroyedPrefabs[
                    Random.Range(0, destructable.destroyedPrefabs.Count)
                ];

                Instantiate(destroyedPrefab, transform.position, transform.rotation);
            }

            // Spawn loot separately
            SpawnMinableLoot();

            // Play destruction feedback
            Health.DeathMMFeedbacks?.PlayFeedbacks(transform.position);

            // Remove the object from the scene
            Destroy(gameObject);
        }

        void SpawnMinableLoot()
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
