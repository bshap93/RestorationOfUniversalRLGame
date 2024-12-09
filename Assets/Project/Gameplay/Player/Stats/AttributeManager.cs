using MoreMountains.Tools;
using Project.Core.CharacterCreation;
using UnityEngine;

namespace Project.Gameplay.Player.Stats
{
    public class AttributeManager : MonoBehaviour, MMEventListener<MMGameEvent>
    {
        public int Strength;
        public int Agility;
        public int Endurance;
        public int Intelligence;
        public int Intuition;

        public int UnusedAttributePoints;

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(MMGameEvent eventType)
        {
            if (eventType.EventName == "PlayerLevelChanged")
            {
                var newLevel = eventType.IntParameter;
                Debug.Log($"PlayerStats: Player leveled up to {newLevel}.");
                OnPlayerLevelUp(newLevel);
                // Do something with the new level
            }
        }


        public void Initialize(CharacterCreationData creationData, PlayerStats playerStats)
        {
            Strength = creationData.attributes.strength;
            Agility = creationData.attributes.agility;
            Endurance = creationData.attributes.endurance;
            Intelligence = creationData.attributes.intelligence;
            Intuition = creationData.attributes.intuition;
        }

        /// <summary>
        ///     Called whenever the player levels up.
        /// </summary>
        /// <param name="newLevel">The new player level.</param>
        void OnPlayerLevelUp(int newLevel)
        {
            Debug.Log($"Player leveled up to Level {newLevel}");

            // Award attribute points at specific levels (e.g., every 2 levels)
            if (ShouldAwardAttributePoint(newLevel))
            {
                UnusedAttributePoints++;
                Debug.Log($"Player awarded an attribute point! Total available points: {UnusedAttributePoints}");
            }
        }

        /// <summary>
        ///     Determines if the player should receive an attribute point at this level.
        ///     Example logic: Award a point every 2 levels.
        /// </summary>
        /// <param name="level">The current player level.</param>
        /// <returns>True if an attribute point should be awarded.</returns>
        bool ShouldAwardAttributePoint(int level)
        {
            // Award a point every 2 levels (2, 4, 6, 8, 10, etc.)
            return level % 2 == 0;
        }

        // Set attributes  
        public void SetAttributes(CharacterStats stats)
        {
            Strength = stats.strength;
            Agility = stats.agility;
            Endurance = stats.endurance;
            Intelligence = stats.intelligence;
            Intuition = stats.intuition;
        }


        public void DisplayStats()
        {
            Debug.Log(
                $"Attributes - Strength: {Strength}, Agility: {Agility}, Endurance: {Endurance}, Intelligence: {Intelligence}, Intuition: {Intuition}");
        }
    }
}
