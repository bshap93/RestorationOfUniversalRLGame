using System;
using System.Collections.Generic;
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
        [SerializeField] Loot _loot;

        public bool ShouldDestroy;
        [FormerlySerializedAs("_colliders")] [SerializeField]
        List<Collider> colliders;
        [FormerlySerializedAs("_renderers")] [SerializeField]
        List<Renderer> renderers;
        GameObject _brokenPrefab;
        bool _isBeingDestroyed;
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

            // colliders = GetComponents<Collider>().ToList();
            // renderers = GetComponents<Renderer>().ToList();
            //
            // colliders.Add(GetComponentInChildren<Collider>());
            // renderers.Add(GetComponentInChildren<Renderer>());

            // On death
            Health.OnDeath += OnDeath;


            if (destructible.destroyedPrefab != null) _brokenPrefab = destructible.destroyedPrefab;
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
            // drop any loot
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

            if (ShouldDestroy)
                Destroy(gameObject, 0.1f);
            else
                DisableRendererAndCollider();
        }

        void DisableRendererAndCollider()
        {
            if (colliders != null)
            {
                foreach (var collider1 in colliders)
                    collider1.enabled = false;

                Debug.Log("Collider disabled");
            }

            if (renderers != null)
            {
                foreach (var renderer1 in renderers)
                    renderer1.enabled = false;

                Debug.Log("Renderer disabled");
            }
        }

        void SaveDestroyedObject(string uniqueID)
        {
            DestructibleManager.SaveDestroyedObject(uniqueID, true);
            Debug.Log("Saved destroyed object: " + uniqueID);
        }
    }
}
