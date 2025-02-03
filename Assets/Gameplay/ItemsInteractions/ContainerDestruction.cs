using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Gameplay.ItemsInteractions
{
    public class ContainerDestruction : MonoBehaviour
    {
        public GameObject brokenBarrelPrefab;
        public GameObject deathFeedbackPrefab;
        public float transitionDuration = 0.5f; // Duration for fading effect

        Health _health;
        Loot _loot; // Reference to the Loot component
        Renderer _renderer;


        void Awake()
        {
            _health = GetComponent<Health>();
            _renderer = GetComponent<Renderer>();
            _loot = GetComponent<Loot>();
            _health.OnDeath += OnDeath;
        }

        void OnDestroy()
        {
            _health.OnDeath -= OnDeath;
        }

        void OnDeath()
        {
            // Spawn the temporary feedback object at the barrel's position
            Instantiate(deathFeedbackPrefab, transform.position, transform.rotation);

            // Instantiate the broken barrel at the same position
            Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

            // Spawn loot at the barrel's position
            if (_loot != null)
                _loot.SpawnLoot();

            // Destroy the original barrel
            Destroy(gameObject);
        }
    }
}
