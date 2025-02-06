using UnityEngine;

namespace Gameplay.ItemsInteractions.Informable
{
    public class InformableItem : ScriptableObject
    {
        public string InformationTitle;
        [TextArea(3, 10)] public string InformationDescription;
    }
}
