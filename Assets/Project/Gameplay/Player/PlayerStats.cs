// PlayerStats.cs

using System;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using Project.Core.CharacterCreation;
using Project.Gameplay.Player.Health;
using Project.Gameplay.Player.Stats;
using TopDownEngine.Common.Scripts.Characters.CharacterAbilities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Player
{
    [Serializable]
    public class PlayerStats : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public XpManager XpManager;
        public AttributeManager AttributeManager;
        // Base Stats
        // [SerializeField] float maxHealth;
        // [SerializeField] float currentHealth;
        [FormerlySerializedAs("moveSpeed")] [SerializeField]
        float moveSpeedMult;
        [SerializeField] float attackPower;
        [FormerlySerializedAs("defense")] [SerializeField]
        float damageMult;
        [SerializeField] bool overrideAutoHealth;

        [SerializeField] float baseMoveSpeed = 5f;
        [SerializeField] float baseRunSpeed = 7f;
        [SerializeField] float baseMaxStamina = 100f;

        // Character creation data for reference
        [SerializeField] string playerClass; // Stores the class name
        [SerializeField] List<string> chosenTraits; // Stores the trait names
        [FormerlySerializedAs("Health")] public float health;
        public CharacterStats characterStats;


        public int playerCurrency;

        // Runtime references to ScriptableObjects for class and traits
        StartingClass startingClass;
        List<CharacterTrait> traits = new();

        public int AttackPower => (int)attackPower;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }


        public void OnMMEvent(MMGameEvent @event)
        {
            if (@event.EventName == "EnemyDeathXP")
            {
                var xpAwarded = @event.IntParameter; // Extract the XP from the event
                XpManager.AddExperience(xpAwarded);


                NotifyStatUpdates();
            }
        }

        // Event that triggers whenever currency changes
        public event Action<int> OnCurrencyChanged;


        public event Action OnStatsUpdated;


        public void Initialize(CharacterCreationData creationData)
        {
            // Store names for reference
            playerClass = creationData.selectedClassName;
            chosenTraits = new List<string>(creationData.selectedTraitNames);

            XpManager.Initialize();

            // Load StartingClass and CharacterTrait ScriptableObjects
            LoadStartingClass(playerClass);
            LoadTraits(chosenTraits);

            // Assign attributes directly from creation data
            AttributeManager.Initialize(creationData, this);
            // Set class and base stats
            ApplyBaseStatsFromClass();
            ApplyAttributesToBaseStats();

            // Initialize current health to max health
            // currentHealth = maxHealth;
            var playerHealth = gameObject.GetComponent<HealthAlt>();

            if (playerHealth != null)
            {
                if (overrideAutoHealth)
                {
                    playerHealth.CurrentHealth = health;
                    playerHealth.MaximumHealth = health;
                    playerHealth.InitialHealth = health;
                }
                else
                {
                    playerHealth.CurrentHealth = playerHealth.MaximumHealth;
                }
            }

            XpManager.playerExperiencePoints = 0;
            XpManager.playerCurrentLevel = 1;
            playerCurrency = 0;
        }

        void LoadStartingClass(string className)
        {
            startingClass = Resources.Load<StartingClass>($"Classes/{className}");
            if (startingClass == null) Debug.LogWarning($"Class {className} not found in Resources.");
        }

        void LoadTraits(List<string> traitNames)
        {
            traits = new List<CharacterTrait>();
            foreach (var traitName in traitNames)
            {
                var trait = Resources.Load<CharacterTrait>($"Traits/{traitName}");
                if (trait != null)
                    traits.Add(trait);
                else
                    Debug.LogError($"Trait {traitName} not found in Resources.");
            }
        }


        void ApplyBaseStatsFromClass()
        {
            if (startingClass == null)
            {
                Debug.LogWarning("Starting class is null; cannot apply base stats.");
                return;
            }

            var playerHealth = gameObject.GetComponent<HealthAlt>();
            // Apply base stats from the class
            if (!overrideAutoHealth)
                if (startingClass.baseStats.TryGetValue(StatType.Endurance, out var enduranceBase))
                    playerHealth.MaximumHealth = enduranceBase * 10;

            if (startingClass.baseStats.TryGetValue(StatType.Agility, out var agilityBase))
                moveSpeedMult = agilityBase * 0.5f;

            if (startingClass.baseStats.TryGetValue(StatType.Strength, out var strengthBase))
                attackPower = strengthBase * 2;

            if (startingClass.baseStats.TryGetValue(StatType.Endurance, out var defenseBase))
                damageMult = defenseBase;
        }

        void ApplyAttributesToBaseStats()
        {
            if (!overrideAutoHealth)
            {
                var playerHealth = gameObject.GetComponent<HealthAlt>();
                playerHealth.MaximumHealth = AttributeManager.Endurance * 2 + 20;

                if (playerHealth != null)
                {
                    playerHealth.InitialHealth = playerHealth.MaximumHealth;
                    // currentHealth = maxHealth;
                    playerHealth.CurrentHealth = playerHealth.InitialHealth;
                    // playerHealth.CurrentHealth = currentHealth;
                }
            }

            // Modify base stats by adding attribute bonuses
            moveSpeedMult = 1 + (AttributeManager.Agility - 2) * 0.05f;
            attackPower += AttributeManager.Strength * 2;
            damageMult = 0.9f + AttributeManager.Endurance * 0.05f;


            var damageResistance = gameObject.GetComponent<DamageResistanceProcessor>().DamageResistanceList[0];
            if (damageResistance != null) damageResistance.DamageMultiplier = damageMult;

            var characterMovement = gameObject.GetComponentInChildren<CharacterMovement>();
            var characterRun = gameObject.GetComponentInChildren<CharacterRun>();
            // 6 is the base movement speed for the character, multiplied by the moveSpeedMult
            if (characterMovement != null) characterMovement.WalkSpeed = moveSpeedMult * baseMoveSpeed;
            if (characterRun != null) characterRun.RunSpeed = moveSpeedMult * baseRunSpeed;

            // Debug.Log(
            //     $"Attributes applied to base stats: MaxHealth={maxHealth}, MoveSpeed={moveSpeedMult}, AttackPower={attackPower}, Defense={damageMult}");
        }

        public void ApplyTraits()
        {
            foreach (var trait in traits)
            {
                foreach (var modifier in trait.statModifiers)
                    if (modifier.type == CharacterTrait.ModifierType.Additive)
                    {
                        if (modifier.statName == "moveSpeed") moveSpeedMult += modifier.value;
                        else if (modifier.statName == "defense") damageMult += modifier.value;
                    }

                Debug.Log($"Trait applied: {trait.traitName}");
            }
        }


        public void AddCoins(int currency)
        {
            playerCurrency += currency;

            // Trigger the event to notify listeners

            OnCurrencyChanged?.Invoke(playerCurrency);
            NotifyStatUpdates();
        }

        /// <summary>
        ///     Handles the OnExperienceChanged event from XpManager.
        /// </summary>
        /// <param name="newXp">The new XP value after the change</param>
        void HandleExperienceChanged(int newXp)
        {
            Debug.Log($"PlayerStats: Player experience updated to {newXp}.");
            NotifyStatUpdates();
        }


        void NotifyStatUpdates()
        {
            OnStatsUpdated?.Invoke();
        }
    }
}
