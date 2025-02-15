using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.ComposedItem.Demo.Scripts
{
    public class RecoverHealthComponent : InventoryItem, IOverridable
    {
        [SerializeField] float Health;
        IOverride IOverridable.NewOverride()
        {
            return new SerializedComponent(this);
        }
        public override bool Use(string playerID)
        {
            Debug.Log("Recovered " + (int)Health + " HP");
            return true;
        }

        [Serializable]
        class SerializedComponent : IOverride
        {
            [SerializeField] float Health;
            public SerializedComponent(RecoverHealthComponent component)
            {
                Save(component);
            }
            IOverridable IOverride.Apply(IOverridable overridable)
            {
                Load((RecoverHealthComponent)overridable);
                return overridable;
            }
            public void Save(RecoverHealthComponent component)
            {
                Health = component.Health;
            }
            public void Load(RecoverHealthComponent component)
            {
                component.Health = Health;
            }
        }
    }
}
