using System;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
using Gameplay.Player.Inventory;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class ContainerDestruction : MonoBehaviour
    {
        public GameObject brokenBarrelPrefab;
        public GameObject deathFeedbackPrefab;
        public string UniqueID;
        public Destructable destructable;

        Health _health;
        bool _isBeingDestroyed;
        SaveableLoot _loot;
        Renderer _renderer;

        void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
                Debug.Log($"Generated new UniqueID for container {gameObject.name}: {UniqueID}");
            }

            _health = GetComponent<Health>();
            _renderer = GetComponent<Renderer>();
            _loot = GetComponent<SaveableLoot>();

            if (_health != null) _health.OnDeath += OnDeath;
        }

        void OnDestroy()
        {
            if (_health != null) _health.OnDeath -= OnDeath;
        }

        void OnDeath()
        {
            if (!_isBeingDestroyed) DestroyContainer(true, true);
        }

        public void DestroyContainer(bool saveState, bool spawnLoot = false)
        {
            if (_isBeingDestroyed) return;
            _isBeingDestroyed = true;

            // Save the destroyed state
            if (saveState) DestructableManager.SaveDestroyedContainer(UniqueID);

            // Spawn visual feedback
            if (deathFeedbackPrefab != null) Instantiate(deathFeedbackPrefab, transform.position, transform.rotation);

            // Spawn broken version
            if (brokenBarrelPrefab != null) Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

            // Only spawn loot if this is an original destruction, not a reconstruction
            if (spawnLoot && _loot != null) _loot.SpawnLoot();

            // Destroy the original container
            Destroy(gameObject);
        }
    }
}
