using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class StatusResourceUI : MonoBehaviour
    {
        public MMProgressBar HealthBar;
        public MMProgressBar PrimaryBar;
        public MMProgressBar SecondaryBar;
        [SerializeField] MagicSystem _magicSystem;

        float _lastHealth;
        float _lastPrimaryResource;
        float _lastSecondaryResource;

        void Update()
        {
            if (_magicSystem == null) return;

            // Check and update Health bar
            var currentHealth = _magicSystem.GetComponent<Health>().CurrentHealth;
            if (!Mathf.Approximately(currentHealth, _lastHealth))
            {
                _lastHealth = currentHealth;
                UpdateBar(HealthBar, currentHealth, _magicSystem.GetComponent<Health>().MaximumHealth);
            }

            // Check and update Primary Resource bar
            var currentPrimary = _magicSystem.PrimaryResource.CurrentResource;
            if (!Mathf.Approximately(currentPrimary, _lastPrimaryResource))
            {
                _lastPrimaryResource = currentPrimary;
                UpdateBar(PrimaryBar, currentPrimary, _magicSystem.PrimaryResource.MaxResource);
            }

            // Check and update Secondary Resource bar
            var currentSecondary = _magicSystem.SecondaryResource.CurrentResource;
            if (!Mathf.Approximately(currentSecondary, _lastSecondaryResource))
            {
                _lastSecondaryResource = currentSecondary;
                UpdateBar(SecondaryBar, currentSecondary, _magicSystem.SecondaryResource.MaxResource);
            }
        }

        public void SetMagicSystem(MagicSystem magicSystem)
        {
            _magicSystem = magicSystem;

            // Initialize values to avoid unnecessary updates
            _lastHealth = _magicSystem.GetComponent<Health>().CurrentHealth;
            _lastPrimaryResource = _magicSystem.PrimaryResource.CurrentResource;
            _lastSecondaryResource = _magicSystem.SecondaryResource.CurrentResource;

            UpdateAllBars(); // Set initial values
        }

        void UpdateBar(MMProgressBar bar, float currentValue, float maxValue)
        {
            if (bar != null) bar.UpdateBar(currentValue, 0f, maxValue);
        }

        void UpdateAllBars()
        {
            if (_magicSystem == null) return;

            UpdateBar(HealthBar, _lastHealth, _magicSystem.GetComponent<Health>().MaximumHealth);
            UpdateBar(PrimaryBar, _lastPrimaryResource, _magicSystem.PrimaryResource.MaxResource);
            UpdateBar(SecondaryBar, _lastSecondaryResource, _magicSystem.SecondaryResource.MaxResource);
        }
    }
}
