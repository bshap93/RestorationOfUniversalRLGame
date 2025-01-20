using System;
using MoreMountains.Tools;
using Project.Gameplay.Interactivity.Items;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomizedDamageAttackUseComponent : InventoryItem, IJsonSerializable, IInitializable, IOverridable
{
    static readonly SerializedComponent Serialized = new();
    [HideInInspector] public float Damage;
    [MMVector("Min", "Max")] public Vector2 DamageRange = new(10, 50);
    bool _initialized;
    void IInitializable.Initialize()
    {
        if (!_initialized) Damage = Random.Range(DamageRange.x, DamageRange.y);
    }
    void IInitializable.Initialized()
    {
        _initialized = true;
    }
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
        Debug.Log("Performed an attack for " + (int)Damage + " damage");
        return true;
    }

    [Serializable]
    protected class SerializedComponent : IOverride
    {
        [HideInInspector] [SerializeField] float Damage;
        [SerializeField] [MMVector("Min", "Max")]
        Vector2 DamageRange;
        public SerializedComponent()
        {
        }
        public SerializedComponent(RandomizedDamageAttackUseComponent component)
        {
            Save(component);
        }
        IOverridable IOverride.Apply(IOverridable overridable)
        {
            Load((RandomizedDamageAttackUseComponent)overridable);
            return overridable;
        }
        public void Save(RandomizedDamageAttackUseComponent component)
        {
            Damage = component.Damage;
            DamageRange = component.DamageRange;
        }
        public void Load(RandomizedDamageAttackUseComponent component)
        {
            component.Damage = Damage;
            component.DamageRange = DamageRange;
        }
    }
}
