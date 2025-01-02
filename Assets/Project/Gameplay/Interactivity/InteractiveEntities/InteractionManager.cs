using System.Collections.Generic;
using UnityEngine;

namespace Project.Gameplay.Interactivity.InteractiveEntities
{
    public class InteractionManager : MonoBehaviour
    {
        private IInteractable currentTarget;
        private List<IInteractable> nearbyInteractables = new List<IInteractable>();
    
        private void UpdateInteractionTarget()
        {
            // Find highest priority interactable in range
            IInteractable bestTarget = null;
            float highestPriority = float.MinValue;
        
            foreach (var interactable in nearbyInteractables)
            {
                if (interactable is IPreviewable previewable)
                {
                    float priority = previewable.GetPreviewPriority();
                    if (priority > highestPriority)
                    {
                        highestPriority = priority;
                        bestTarget = interactable;
                    }
                }
            }
        
            // Update current target
            if (currentTarget != bestTarget)
            {
                if (currentTarget is IPreviewable oldPreview)
                    oldPreview.HidePreview();
                
                currentTarget = bestTarget;
            
                if (currentTarget is IPreviewable newPreview)
                    newPreview.ShowPreview();
            }
        }
    }
}
