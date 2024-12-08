// PlayerStats.cs

using System;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using Project.Core.CharacterCreation;
using Project.Gameplay.Player.Health;
using UnityEngine;
using UnityEngine.Serialization;

namespace Project.Gameplay.Player
{
    [Serializable]
    public class PlayerStats : MonoBehaviour
    {
        // Base Stats
        // [SerializeField] float maxHealth;
        // [SerializeField] float currentHealth;
        [FormerlySerializedAs("moveSpeed")] [SerializeField]
        float moveSpeedMult;
        [SerializeField] float attackPower;
        [FormerlySerializedAs("defense")] [SerializeField]
        float damageMult;
        [SerializeField] bool overrideAutoHealth;

        // Attributes assigned by the player
        [SerializeField] int strength;
        [SerializeField] int agility;
        [SerializeField] int endurance;
        [SerializeField] int intelligence;
        [SerializeField] int intuition;

        // Character creation data for reference
        [SerializeField] string playerClass; // Stores the class name
        [SerializeField] List<string> chosenTraits; // Stores the trait names
        public float Health;

        public int playerExperiencePoints;
        public int playerCurrentLevel;

        public int playerCurrency;

        // Runtime references to ScriptableObjects for class and traits
        StartingClass startingClass;
        List<CharacterTrait> traits = new();

        public int Strength => strength;
        public int AttackPower => (int)attackPower;

        // Event that triggers whenever currency changes
        public event Action<int> OnCurrencyChanged;

        public event Action OnStatsUpdated;

        public void Initialize(CharacterCreationData creationData)
        {
            // Store names for reference
            playerClass = creationData.selectedClassName;
            chosenTraits = new List<string>(creationData.selectedTraitNames);

            // Load StartingClass and CharacterTrait ScriptableObjects
            LoadStartingClass(playerClass);
            LoadTraits(chosenTraits);

            // Assign attributes directly from creation data
            SetAttributes(creationData);

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
                    playerHealth.CurrentHealth = Health;
                    playerHealth.MaximumHealth = Health;
                    playerHealth.InitialHealth = Health;
                }
                else
                {
                    playerHealth.CurrentHealth = playerHealth.MaximumHealth;
                }
            }

            playerExperiencePoints = 0;
            playerCurrentLevel = 1;
            playerCurrency = 0;
        }

        void LoadStartingClass(string className)
        {
            startingClass = Resources.Load<StartingClass>($"Classes/{className}");
            if (startingClass == null) Debug.LogError($"Class {className} not found in Resources.");
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

        void SetAttributes(CharacterCreationData creationData)
        {
            strength = creationData.attributes.strength;
            agility = creationData.attributes.agility;
            endurance = creationData.attributes.endurance;
            intelligence = creationData.attributes.intelligence;
            intuition = creationData.attributes.intuition;
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
                playerHealth.MaximumHealth = endurance * 2 + 20;

                if (playerHealth != null)
                {
                    playerHealth.InitialHealth = playerHealth.MaximumHealth;
                    // currentHealth = maxHealth;
                    playerHealth.CurrentHealth = playerHealth.InitialHealth;
                    // playerHealth.CurrentHealth = currentHealth;
                }
            }

            // Modify base stats by adding attribute bonuses
            moveSpeedMult = 1 + (agility - 2) * 0.05f;
            attackPower += strength * 2;
            damageMult = 0.9f + endurance * 0.05f;


            var damageResistance = gameObject.GetComponent<DamageResistanceProcessor>().DamageResistanceList[0];
            if (damageResistance != null) damageResistance.DamageMultiplier = damageMult;

            var characterMovement = gameObject.GetComponent<CharacterMovement>();
            // 6 is the base movement speed for the character, multiplied by the moveSpeedMult
            if (characterMovement != null) characterMovement.WalkSpeed = moveSpeedMult * 6;

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

        public void DisplayStats()
        {
            // Debug.Log(
            //     $"Class: {playerClass}, Max Health: {maxHealth}, Current Health: {currentHealth}, Move Speed: {moveSpeedMult}, Attack Power: {attackPower}, Defense: {damageMult}");
            //
            Debug.Log(
                $"Attributes - Strength: {strength}, Agility: {agility}, Endurance: {endurance}, Intelligence: {intelligence}, Intuition: {intuition}");

            Debug.Log($"Chosen Traits: {string.Join(", ", chosenTraits)}");
        }

        public void LevelUp()
        {
            Debug.Log("Player leveled up!");
            NotifyStatUpdates();
        }

        public void AddExperience(int experience)
        {
            playerExperiencePoints += experience;
            Debug.Log($"Player gained {experience} experience points.");
            NotifyStatUpdates();
        }

        public void AddCoins(int currency)
        {
            playerCurrency += currency;

            // Trigger the event to notify listeners

            OnCurrencyChanged?.Invoke(playerCurrency);
            Debug.Log($"Player gained {currency} currency.");
            NotifyStatUpdates();
        }


        void NotifyStatUpdates()
        {
            OnStatsUpdated?.Invoke();
        }
    }
}
