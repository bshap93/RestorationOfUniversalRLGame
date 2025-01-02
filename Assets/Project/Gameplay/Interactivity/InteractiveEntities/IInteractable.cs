using System;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Interactivity.InteractiveEntities
{
    [Serializable]
    public enum InteractionType
    {
        Item,
        CraftingStation
        // Expandable for future types
    }


    public interface IInteractable
    {
        bool CanInteract(Character playerCharacter);
        void OnInteract(Character playerCharacter);
        InteractionType GetInteractionType();
        Transform GetTransform(); // For positioning previews/UI
    }
}
