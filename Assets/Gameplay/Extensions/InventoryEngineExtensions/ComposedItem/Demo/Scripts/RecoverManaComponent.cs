using System;
using MoreMountains.InventoryEngine;
using UnityEngine;

namespace Gameplay.Extensions.InventoryEngineExtensions.ComposedItem.Demo.Scripts
{
    public class RecoverManaComponent : InventoryItem, IOverridable
    {
        [SerializeField] float Mana;
        IOverride IOverridable.NewOverride()
        {
            return new SerializedComponent(this);
        }
        public override bool Use(string playerID)
        {
            Debug.Log("Recovered " + (int)Mana + " MP");
            return true;
        }

        [Serializable]
        class SerializedComponent : IOverride
        {
            [SerializeField] float Mana;
            public SerializedComponent(RecoverManaComponent component)
            {
                Save(component);
            }
            IOverridable IOverride.Apply(IOverridable overridable)
            {
                Load((RecoverManaComponent)overridable);
                return overridable;
            }
            public void Save(RecoverManaComponent component)
            {
                Mana = component.Mana;
            }
            public void Load(RecoverManaComponent component)
            {
                component.Mana = Mana;
            }
        }
    }
}
