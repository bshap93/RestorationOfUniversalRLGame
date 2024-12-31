using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

namespace Project.Gameplay.Magic
{
    public class StatusResourceSystemsInitializer : MonoBehaviour, MMEventListener<TopDownEngineEvent>
    {
        public StatusResourceUI ResourceUI; // Reference to the UI component

        void OnEnable()
        {
            this.MMEventStartListening();
        }

        void OnDisable()
        {
            this.MMEventStopListening();
        }

        public void OnMMEvent(TopDownEngineEvent engineEvent)
        {
            if (engineEvent.EventType == TopDownEngineEventTypes.SpawnComplete)
            {
                var player = engineEvent.OriginCharacter?.gameObject;
                if (player != null) LinkPlayerToUI(player);
            }
        }

        void LinkPlayerToUI(GameObject player)
        {
            var magicSystem = player.GetComponent<MagicSystem>();
            if (magicSystem != null && ResourceUI != null) ResourceUI.SetMagicSystem(magicSystem);
        }
    }
}
