using System;

namespace Project.Gameplay.Interactivity.InteractiveEntities
{
    [Serializable]
    public enum PreviewType
    {
        ItemPickup,
        CraftingStation
        // Expandable for future preview types
    }

    public interface IPreviewable
    {
        void ShowPreview();
        void HidePreview();
        PreviewType GetPreviewType();
        float GetPreviewPriority(); // Higher priority previews override lower ones 
    }
}
