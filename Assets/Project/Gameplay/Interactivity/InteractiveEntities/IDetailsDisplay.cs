using UnityEngine;

namespace Project.Gameplay.Interactivity.InteractiveEntities
{
    public interface IDetailsDisplay
    {
        void DisplayDetails(IPreviewable previewable);
        void Hide();
        CanvasGroup CanvasGroup { get; }
    }
}
