using MoreMountains.TopDownEngine;
using Project.Gameplay.Player;
using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    public class WeaponStatHandler : MonoBehaviour
    {
        PlayerStats _playerStats;
        public int StrengthModifier { get; private set; }

        void Awake()
        {
            FindPlayerStats();
        }

        void OnDestroy()
        {
            // Unsubscribe to avoid memory leaks
            if (_playerStats != null) _playerStats.OnStatsUpdated -= UpdateStrengthModifier;
        }

        void FindPlayerStats()
        {
            // Search upwards in the hierarchy
            _playerStats = GetComponentInParent<PlayerStats>();

            // If not found in parents, search the scene
            if (_playerStats == null) _playerStats = FindObjectOfType<PlayerStats>();

            if (_playerStats != null)
            {
                SubscribeToStatUpdates();
                UpdateStrengthModifier();
            }
            else
            {
                Debug.LogWarning("PlayerStats not found! Weapon strength modifier will remain default.");
            }
        }

        void SubscribeToStatUpdates()
        {
            // Example: Subscribe to an event for stat changes
            _playerStats.OnStatsUpdated += UpdateStrengthModifier;
        }

        void UpdateStrengthModifier()
        {
            if (_playerStats != null)
            {
                StrengthModifier = _playerStats.Strength;
                var meleeWeapon = gameObject.GetComponent<MeleeWeapon>();
                if (meleeWeapon != null)
                {
                    meleeWeapon.MinDamageCaused = 8 + _playerStats.AttackPower * 0.5f;
                    meleeWeapon.MaxDamageCaused = 46 + _playerStats.AttackPower * 1f;
                }

                Debug.Log($"Strength modifier updated to: {StrengthModifier}");
            }
        }
    }
}
