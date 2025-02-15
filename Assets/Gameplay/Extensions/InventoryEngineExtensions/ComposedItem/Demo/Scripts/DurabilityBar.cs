using System;
using Gameplay.Extensions.InventoryEngineExtensions.ComposedItem.Demo.Scripts;
using MoreMountains.Tools;
using UnityEngine;

public class DurabilityBar : MonoBehaviour
{
    MMProgressBar _bar;
    [NonSerialized] public DurabilityUseComponent Component;
    void Awake()
    {
        _bar = GetComponent<MMProgressBar>();
    }
    void Update()
    {
        if (Component) _bar.SetBar01(Component.Durability);
    }
}
