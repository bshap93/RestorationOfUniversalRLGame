using System;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
using Gameplay.Player.Inventory;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class BaseDestructable : MonoBehaviour
    {
        public string UniqueID;
        public Destructable destructable;
        [SerializeField] protected GameObject BrokenPrefab;
        protected Health Health;
        protected bool IsBeingDestroyed;
        protected SaveableLoot Loot;
        protected Renderer Renderer;

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
                Debug.Log($"Generated new UniqueID for Destructable Object {gameObject.name}: {UniqueID}");
            }

            Health = GetComponent<Health>();
            Renderer = GetComponent<Renderer>();
            Loot = GetComponent<SaveableLoot>();


            if (Health != null) Health.OnDeath += OnDeath;
        }


        protected void OnDeath()
        {
            if (!IsBeingDestroyed) DestroyDestructableObject(true, true);
        }

        public void DestroyDestructableObject(bool saveState, bool spawnLoot = false)
        {
            if (IsBeingDestroyed) return;
            if (destructable == null) Debug.LogWarning("No destructable ScriptableObject found on " + gameObject.name);
            IsBeingDestroyed = true;

            // Save the destroyed state
            // if (saveState) DestructableManager.SaveDestroyedContainer(UniqueID);

            // Spawn broken version
            if (BrokenPrefab != null) Instantiate(BrokenPrefab, transform.position, transform.rotation);

            // Only spawn loot if this is an original destruction, not a reconstruction
            if (spawnLoot && Loot != null) Loot.SpawnLoot();

            // Destroy the original container
            Destroy(gameObject);
        }
    }
}