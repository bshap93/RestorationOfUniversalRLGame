using System.Collections.Generic;
using MoreMountains.Tools;
using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    public class MeleeWeaponExtension : MonoBehaviour
    {
        // Enum for MeleeWeapon types
        public enum MeleeWeaponType
        {
            Sword,
            Axe
        }

        [MMInspectorGroup("Particle Effects", true)]
        public List<ParticleSystem> AttackParticles; // List of attack particles 
        public List<ParticleSystem> HitParticles; // List of hit particles
        [SerializeField] EffectRoots _playerEffectsRoot; // Cache for player's effects root

        [MMInspectorGroup("Attack Variations", true)]
        public List<string> AttackAnimations; // Animation triggers in the Animator
        public bool UseRandomAnimation = true; // Toggle for random animations
        int _currentAnimationIndex = 0;

        protected void Start()
        {
            _playerEffectsRoot = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<EffectRoots>();
        }
    }
}
