using MoreMountains.Tools;
using Project.Gameplay.Interactivity.InteractiveEntities;
using Project.UI.HUD;

namespace Project.Gameplay.Interactivity.Items
{
    public class PreviewableInventoryItem : InventoryItem, IPreviewable
    {
        public virtual void ShowPreview()
        {
            MMEventManager.TriggerEvent(
                new PreviewEvent
                {
                    EventType = PreviewEventType.ShowPreview,
                    Previewable = this
                });
        }

        public virtual void HidePreview()
        {
            MMEventManager.TriggerEvent(
                new PreviewEvent
                {
                    EventType = PreviewEventType.HidePreview,
                    Previewable = this
                });
        }

        public virtual PreviewType GetPreviewType()
        {
            return PreviewType.ItemPickup;
        }
        public virtual float GetPreviewPriority()
        {
            return 1.0f;
        }
    }
}
