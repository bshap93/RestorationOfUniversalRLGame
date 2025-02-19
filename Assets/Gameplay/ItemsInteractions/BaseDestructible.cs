using System;
using Gameplay.Events;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
using Gameplay.Player.Inventory;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay.ItemsInteractions
{
    public class BaseDestructible : MonoBehaviour
    {
        public string UniqueID;
        [FormerlySerializedAs("destructable")] public Destructible destructible;
        GameObject _brokenPrefab;
        bool _isBeingDestroyed;
        protected Health Health;
        protected SaveableLoot Loot;


        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
                Debug.Log($"Generated new UniqueID for Destructable Object {gameObject.name}: {UniqueID}");
            }

            Health = GetComponent<Health>();
            Loot = GetComponent<SaveableLoot>();

            // On death
            Health.OnDeath += OnDeath;
        }

        void Start()
        {
            if (DestructibleManager.IsObjectDestroyed(UniqueID)) Destroy(gameObject);
            if (destructible.destroyedPrefabs.Count > 0) _brokenPrefab = destructible.destroyedPrefabs[0];
        }


        void OnDeath()
        {
            enabled = false;
            Debug.Log("Destructible object destroyed: " + gameObject.name);
            FinishDestroying();
        }

        void FinishDestroying()
        {
            SaveDestroyedObject(UniqueID);
            DestructibleEvent.Trigger("DestructibleDestroyed", destructible, transform);

            DestructibleManager.DestroyedObjects.Add(UniqueID);
            Destroy(gameObject, 0.1f);
        }

        void SaveDestroyedObject(string uniqueID)
        {
            DestructibleManager.SaveDestroyedObject(uniqueID, true);
        }
    }
}
