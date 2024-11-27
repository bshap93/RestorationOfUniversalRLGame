using System.Collections.Generic;
using MoreMountains.Tools;
using Project.Core.Graphical.Effects;
using UnityEngine;

namespace Project.Gameplay.Combat.Weapons
{
    public class MeleeWeaponExtension : MonoBehaviour
    {
        [MMInspectorGroup("Particle Effects", true)]
        public List<GameObject> AttackParticlesPrefabs; // List of attack particles 
        public List<GameObject> HitParticlesPrefabs; // List of hit particles
        [SerializeField] EffectRoots _playerEffectsRoot; // Cache for player's effects root
        public MeleeWeaponType MWWeaponType; // Weapon type

        [MMInspectorGroup("Attack Variations", true)]
        public List<string> AttackAnimations; // Animation triggers in the Animator
        public bool UseRandomAnimation = true; // Toggle for random animations
        int _currentAnimationIndex = 0;

        protected void Start()
        {
            _playerEffectsRoot = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<EffectRoots>();
        }

        public void PlayBasicAttackParticleEffect()
        {
            // Get the current effect root
            var effectRoot = MWWeaponType == MeleeWeaponType.Sword
                ? _playerEffectsRoot.SwordEffectRootsList[0]
                : _playerEffectsRoot.AxeEffectRootsList[0];

            // Instantiate the attack particle effect
            var attackParticle = Instantiate(AttackParticlesPrefabs[0], effectRoot.position, effectRoot.rotation);
            attackParticle.transform.SetParent(effectRoot);

            attackParticle.GetComponent<ParticleSystem>().Play();
        }
    }
}
