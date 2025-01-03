using System;
using MoreMountains.InventoryEngine;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;

public class DurabilityUseComponent : InventoryItem, IJsonSerializable, IOverridable
{
    static readonly SerializedComponent Serialized = new();
    [HideInInspector] public float Durability = 1;
    [Header("Durability")] public float DurabilityLostPerUse = .2f;
    public bool RemoveItemWhenEmpty;
    string IJsonSerializable.Save()
    {
        Serialized.Save(this);
        return JsonUtility.ToJson(Serialized);
    }
    void IJsonSerializable.Load(string json)
    {
        JsonUtility.FromJsonOverwrite(json, Serialized);
        Serialized.Load(this);
    }
    IOverride IOverridable.NewOverride()
    {
        return new SerializedComponent(this);
    }
    public override bool Use(string playerID)
    {
        Durability -= DurabilityLostPerUse;
        if (Durability > 0.0001f) return true;
        Durability = 0;
        foreach (var inventory in Inventory.RegisteredInventories)
            for (var i = 0; i < inventory.Content.Length; i++)
                if (inventory.Content[i] is ComposedItem item && item.Components.Contains(this))
                {
                    Debug.Log(item.ItemName + (RemoveItemWhenEmpty ? " broke" : " is empty"));
                    if (RemoveItemWhenEmpty) inventory.RemoveItem(i, 1);
                    return false;
                }

        return false;
    }

    [Serializable]
    class SerializedComponent : IOverride
    {
        [SerializeField] [HideInInspector] float Durability;
        [SerializeField] float DurabilityLostPerUse;
        [SerializeField] bool RemoveItemWhenEmpty;
        public SerializedComponent()
        {
        }
        public SerializedComponent(DurabilityUseComponent component)
        {
            Save(component);
        }
        IOverridable IOverride.Apply(IOverridable overridable)
        {
            Load((DurabilityUseComponent)overridable);
            return overridable;
        }
        public void Save(DurabilityUseComponent component)
        {
            Durability = component.Durability;
            DurabilityLostPerUse = component.DurabilityLostPerUse;
            RemoveItemWhenEmpty = component.RemoveItemWhenEmpty;
        }
        public void Load(DurabilityUseComponent component)
        {
            component.Durability = Durability;
            component.DurabilityLostPerUse = DurabilityLostPerUse;
            component.RemoveItemWhenEmpty = RemoveItemWhenEmpty;
        }
    }
}
