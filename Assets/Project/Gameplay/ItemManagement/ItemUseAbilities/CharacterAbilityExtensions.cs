using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.ItemManagement.ItemUseAbilities
{
    /// <summary>
    ///     Extends Character class to provide ID-based ability lookup
    /// </summary>
    public static class CharacterAbilityExtensions
    {
        /// <summary>
        ///     Finds an ability of type T with the specified ID
        /// </summary>
        public static T FindAbilityByID<T>(this Character character, int abilityID)
            where T : CharacterAbility, IAbilityID
        {
            var abilities = character.FindAbilities<T>();
            foreach (var ability in abilities)
                if (ability is IAbilityID abilityWithID && abilityWithID.AbilityID == abilityID)
                    return ability;

            return null;
        }
    }

    /// <summary>
    ///     Interface for abilities that can be identified by an ID
    /// </summary>
    public interface IAbilityID
    {
        int AbilityID { get; }
    }

    /// <summary>
    ///     Base class for abilities that need to be identified by ID
    /// </summary>
    public abstract class IdentifiableCharacterAbility : CharacterAbility, IAbilityID
    {
        [Header("Ability Identification")]
        [Tooltip(
            "Unique identifier for this ability instance. Used to target specific abilities when multiple of the same type exist")]
        public int AbilityID = 1;

        int IAbilityID.AbilityID => AbilityID;
    }
}
