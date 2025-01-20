using System;
using MoreMountains.TopDownEngine;

namespace Project.Gameplay.Combat.Weapons
{
    [Serializable]
    public class UnivWeapon : Weapon
    {
        // /// <summary>
        // /// Copies an item into a new one
        // /// </summary>
        public virtual UnivWeapon Copy()
        {
            var cloneName = name;
            var clone = Instantiate(this);
            clone.name = cloneName;
            return clone;
        }
    }
}
