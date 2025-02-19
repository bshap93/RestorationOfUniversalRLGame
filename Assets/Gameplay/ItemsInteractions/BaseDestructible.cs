using System;
using Gameplay.Events;
using Gameplay.ItemManagement.InventoryTypes.Destructables;
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
        Loot _loot;
        int dropAmountMax;
        int dropAmountMin;
        protected Health Health;


        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(UniqueID))
            {
                UniqueID = Guid.NewGuid().ToString();
                Debug.Log($"Generated new UniqueID for Destructable Object {gameObject.name}: {UniqueID}");
            }

            Health = GetComponent<Health>();

            // On death
            Health.OnDeath += OnDeath;


            if (destructible.destroyedPrefab != null) _brokenPrefab = destructible.destroyedPrefab;
            _loot = GetComponent<Loot>();
            dropAmountMin = destructible.dropAmountRange.x;
            dropAmountMax = destructible.dropAmountRange.y;
        }

        void Start()
        {
            if (DestructibleManager.IsObjectDestroyed(UniqueID))
            {
                InitializeDestroyedStateObject();
                Destroy(gameObject);
            }
        }
        void InitializeDestroyedStateObject()
        {
            if (_brokenPrefab == null) return;

            Debug.Log("Initializing destroyed state object");

            var broken = Instantiate(_brokenPrefab, transform.position, transform.rotation);
            broken.transform.parent = transform.parent;
            broken.transform.localScale = transform.localScale;
            broken.transform.rotation = transform.rotation;
            broken.transform.position = transform.position;
            broken.SetActive(true);
            
            
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
            InitializeDestroyedStateObject();
            Destroy(gameObject, 0.1f);
        }

        void SaveDestroyedObject(string uniqueID)
        {
            DestructibleManager.SaveDestroyedObject(uniqueID, true);
        }
    }
}
